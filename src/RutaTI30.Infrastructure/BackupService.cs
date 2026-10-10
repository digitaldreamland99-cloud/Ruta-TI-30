using Microsoft.Data.Sqlite;

namespace RutaTI30.Infrastructure;

/// <summary>Resultado de la validación de una copia de seguridad.</summary>
public sealed record BackupValidation(bool IsValid, int SchemaVersion, IReadOnlyList<string> Errors);

/// <summary>Resultado de una restauración.</summary>
public sealed record RestoreResult(bool Success, string? SafetyBackupPath, IReadOnlyList<string> Errors);

/// <summary>
/// Copias de seguridad y restauración controlada. Una copia solo se restaura si pasa la validación
/// (integridad SQLite + versión de esquema compatible + tablas esperadas) y siempre se conserva
/// una copia del estado previo. No se promete recuperación ante cualquier fallo: ver BACKUP_RESTORE.md.
/// </summary>
public sealed class BackupService
{
    private readonly AppDb _db;
    private readonly string _backupDirectory;

    public BackupService(AppDb db, string backupDirectory)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _backupDirectory = backupDirectory ?? throw new ArgumentNullException(nameof(backupDirectory));
    }

    /// <summary>
    /// Crea una copia consistente con VACUUM INTO (instantánea transaccional) y devuelve su ruta.
    ///
    /// Nota de diseño SQLite + WAL:
    /// - VACUUM INTO usa una conexión propia y produce una copia consolidada del archivo actual,
    ///   por lo que no requiere copiar los archivos auxiliares -wal/-shm.
    /// - Durante la copia, SQLite puede seguir operando en WAL, pero los cambios pendientes del
    ///   archivo actual no se pierden porque la copia se toma del propio archivo de base, no de los
    ///   accesorios. SQLite consolidará el WAL en el momento adecuado.
    /// </summary>
    public string CreateBackup()
    {
        Directory.CreateDirectory(_backupDirectory);

        var fileName = $"ruta-ti-30-{DateTime.Now:yyyyMMdd-HHmmss}.db";
        var path = Path.Combine(_backupDirectory, fileName);
        int suffix = 1;
        while (File.Exists(path))
        {
            path = Path.Combine(_backupDirectory,
                $"ruta-ti-30-{DateTime.Now:yyyyMMdd-HHmmss}-{suffix++}.db");
        }

        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "VACUUM INTO @path;";
        cmd.Parameters.AddWithValue("@path", path);
        cmd.ExecuteNonQuery();
        return path;
    }

    /// <summary>
    /// Valida que el archivo sea una copia utilizable: integridad, versión de esquema y tablas clave.
    /// La validación se abre con una conexión propia en modo solo lectura.
    /// </summary>
    public BackupValidation Validate(string path)
    {
        var errors = new List<string>();
        if (!File.Exists(path))
            return new BackupValidation(false, 0, new[] { "El archivo no existe." });

        try
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadOnly
            }.ToString();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using (var integrity = connection.CreateCommand())
            {
                integrity.CommandText = "PRAGMA integrity_check;";
                var result = integrity.ExecuteScalar() as string;
                if (!string.Equals(result, "ok", StringComparison.OrdinalIgnoreCase))
                    errors.Add($"Integridad SQLite fallida: {result ?? "sin resultado"}.");
            }

            int version;
            using (var versionCmd = connection.CreateCommand())
            {
                versionCmd.CommandText = "PRAGMA user_version;";
                version = Convert.ToInt32(
                    versionCmd.ExecuteScalar(),
                    System.Globalization.CultureInfo.InvariantCulture);
            }

            if (version < 1)
                errors.Add("El archivo no parece una base de datos de Ruta TI 30 (versión de esquema no reconocida).");
            if (version > Migrations.CurrentVersion)
                errors.Add($"La copia usa una versión de esquema más nueva ({version}) que esta aplicación ({Migrations.CurrentVersion}).");

            using (var tables = connection.CreateCommand())
            {
                tables.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'lesson_progress';";
                var count = Convert.ToInt32(
                    tables.ExecuteScalar(),
                    System.Globalization.CultureInfo.InvariantCulture);
                if (count == 0)
                    errors.Add("Falta la tabla «lesson_progress»: no es una copia válida de Ruta TI 30.");
            }

            return new BackupValidation(errors.Count == 0, version, errors);
        }
        catch (Exception ex) when (ex is SqliteException or IOException or UnauthorizedAccessException)
        {
            errors.Add($"No se pudo leer el archivo: {ex.Message}");
            return new BackupValidation(false, 0, errors);
        }
    }

    // Copia los archivos de soporte WAL del archivo origen al destino indicado.
    private static void CopyWalSupportFiles(string sourcePath, string targetBasePath)
    {
        foreach (var suffix in new[] { "-wal", "-shm" })
        {
            var src = sourcePath + suffix;
            if (File.Exists(src))
                File.Copy(src, targetBasePath + suffix, overwrite: true);
        }
    }

    // Elimina los archivos de soporte del archivo indicado.
    private static void DeleteWalSupportFiles(string basePath)
    {
        foreach (var suffix in new[] { "-wal", "-shm" })
        {
            var aux = basePath + suffix;
            if (File.Exists(aux))
                File.Delete(aux);
        }
    }

    /// <summary>
    /// Restaura una copia validada. Si algo falla, la base de datos actual queda como estaba
    /// (se restituye desde la copia de seguridad previa) y se devuelven los errores.
    ///
    /// Nota de diseño importante para SQLite + WAL:
    /// - CreateBackup usa VACUUM INTO sobre una conexión propia, por lo que la copia es una
    ///   instantánea transaccional sin depender de los archivos -wal/-shm en ese momento.
    /// - Por eso la restauración NO elimina -wal/-shm del archivo original antes de copiar el
    ///   respaldo: borrarlos prematuramente descartaría transacciones aún no consolidadas.
    /// - La restauración transfiere los -wal/-shm del respaldo si los tiene, y solo si el archivo
    ///   actual tenía auxiliares y el respaldo no los trae se restauran los actuales.
    /// - Si la restauración falla, se restituye la copia automática del estado previo y también
    ///   se reintentan copiar sus -wal/-shm asociados si los tiene.
    /// - No se asume que eliminar -wal/-shm sea siempre incorrecto: se hace solo cuando la copia
    ///   validada no los trae y la base actual tampoco tenía por qué tenerlos después del reemplazo.
    /// </summary>
    public RestoreResult Restore(string path)
    {
        var validation = Validate(path);
        if (!validation.IsValid)
            return new RestoreResult(false, null, validation.Errors);

        string? safetyBackup = null;
        try
        {
            var activeMain = _db.DatabasePath;
            var activeWal = activeMain + "-wal";
            var activeShm = activeMain + "-shm";
            var activeWalExists = File.Exists(activeWal);
            var activeShmExists = File.Exists(activeShm);

            // 1. Respaldo automático del estado actual antes de tocar nada.
            if (File.Exists(activeMain))
                safetyBackup = CreateBackup();

            // 2. Respaldar los accesorios WAL actuales antes de reemplazar la base.
            string? currentWalBackup = null;
            string? currentShmBackup = null;

            if (activeWalExists)
            {
                currentWalBackup = activeMain + "-wal.bak";
                File.Copy(activeWal, currentWalBackup, overwrite: true);
            }
            if (activeShmExists)
            {
                currentShmBackup = activeMain + "-shm.bak";
                File.Copy(activeShm, currentShmBackup, overwrite: true);
            }

            try
            {
                // 3. Reemplazo controlado de la base actual.
                File.Copy(path, activeMain, overwrite: true);

                // 4. Transferir los accesorios WAL del respaldo si los tiene.
                CopyWalSupportFiles(path, activeMain);

                // 5. Si el respaldo no traía WAL, restaurar los accesorios actuales que había.
                if (!File.Exists(path + "-wal") && currentWalBackup is not null)
                    File.Move(currentWalBackup, activeWal, overwrite: true);
                if (!File.Exists(path + "-shm") && currentShmBackup is not null)
                    File.Move(currentShmBackup, activeShm, overwrite: true);

                // 6. Si quedan accesorios huérfanos del archivo anterior, limpiarlos.
                if (!activeWalExists)
                    DeleteWalSupportFiles(activeMain);

                // 7. Purgar pools para no servir páginas en caché del archivo anterior.
                SqliteConnection.ClearAllPools();

                // 8. Aplicar o reaplicar migraciones sobre la base restaurada.
                _db.Initialize();

                return new RestoreResult(true, safetyBackup, Array.Empty<string>());
            }
            catch
            {
                // Restaurar los accesorios actuales si la etapa 3-5 falló.
                if (currentWalBackup is not null
                    && File.Exists(currentWalBackup)
                    && activeWalExists)
                {
                    try { File.Move(currentWalBackup, activeWal, overwrite: true); }
                    catch { }
                }
                if (currentShmBackup is not null
                    && File.Exists(currentShmBackup)
                    && activeShmExists)
                {
                    try { File.Move(currentShmBackup, activeShm, overwrite: true); }
                    catch { }
                }

                throw;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SqliteException)
        {
            var restoreErrors = new List<string> { $"Error durante la restauración: {ex.Message}" };
            if (safetyBackup is not null && File.Exists(safetyBackup))
            {
                try
                {
                    File.Copy(safetyBackup, _db.DatabasePath, overwrite: true);
                    CopyWalSupportFiles(safetyBackup, _db.DatabasePath);
                    SqliteConnection.ClearAllPools();
                    _db.Initialize();
                    restoreErrors.Add("Se restituyó el estado previo desde la copia de seguridad automática.");
                }
                catch (Exception rollbackEx)
                {
                    restoreErrors.Add($"No se pudo restituir el estado previo: {rollbackEx.Message}");
                }
            }
            return new RestoreResult(false, safetyBackup, restoreErrors);
        }
    }
}
