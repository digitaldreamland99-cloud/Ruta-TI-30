using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Infrastructure;

public class BackupServiceTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly BackupService _service;
    private readonly LearningStore _store;

    public BackupServiceTests()
    {
        _service = new BackupService(_db.Db, Path.Combine(_db.DirectoryPath, "backups"));
        _store = new LearningStore(_db.Db);
    }

    public void Dispose() => _db.Dispose();

    private void GuardarProgreso(string lessonId, LessonState estado) =>
        _store.SaveProgress(new LessonProgress
        {
            LessonId = lessonId,
            State = estado,
            StartedUtc = DateTime.UtcNow
        });

    [Fact]
    public void CreateBackup_GeneraArchivoValido()
    {
        GuardarProgreso("leccion-01", LessonState.Completed);

        var path = _service.CreateBackup();

        Assert.True(File.Exists(path));
        var validacion = _service.Validate(path);
        Assert.True(validacion.IsValid, string.Join(", ", validacion.Errors));
        Assert.Equal(Migrations.CurrentVersion, validacion.SchemaVersion);
    }

    [Fact]
    public void Validate_ArchivoBasura_EsInvalido()
    {
        var corrupto = Path.Combine(_db.DirectoryPath, "corrupto.db");
        File.WriteAllText(corrupto, "esto no es una base de datos sqlite");

        var validacion = _service.Validate(corrupto);

        Assert.False(validacion.IsValid);
        Assert.NotEmpty(validacion.Errors);
    }

    [Fact]
    public void Validate_ArchivoInexistente_EsInvalido()
    {
        var validacion = _service.Validate(
            Path.Combine(_db.DirectoryPath, "no-existe.db"));

        Assert.False(validacion.IsValid);
        Assert.Contains("no existe", validacion.Errors[0]);
    }

    [Fact]
    public void Restore_ArchivoInvalido_NoTocaLosDatosActuales()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var corrupto = Path.Combine(_db.DirectoryPath, "corrupto.db");
        File.WriteAllText(corrupto, "basura");

        var result = _service.Restore(corrupto);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);

        Assert.Equal(
            LessonState.Mastered,
            _store.GetProgress("leccion-01")!.State);
    }

    [Fact]
    public void Restore_CopiaValida_RestauraYConservaCopiaDeSeguridad()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var copia = _service.CreateBackup();

        GuardarProgreso("leccion-01", LessonState.Completed);
        GuardarProgreso("leccion-02", LessonState.InProgress);
        Assert.Equal(2, _store.GetAllProgress().Count);

        var result = _service.Restore(copia);

        Assert.True(result.Success, string.Join(", ", result.Errors));
        Assert.NotNull(result.SafetyBackupPath);
        Assert.True(File.Exists(result.SafetyBackupPath!));

        var validacionPrevia = _service.Validate(result.SafetyBackupPath!);
        Assert.True(validacionPrevia.IsValid);

        var store2 = new LearningStore(_db.Db);
        Assert.Single(store2.GetAllProgress());
        Assert.Equal(
            LessonState.Mastered,
            store2.GetProgress("leccion-01")!.State);

        using var dbPrevia = new AppDb(result.SafetyBackupPath!);
        var storePrevio = new LearningStore(dbPrevia);
        Assert.Equal(2, storePrevio.GetAllProgress().Count);
        Assert.Equal(
            LessonState.Completed,
            storePrevio.GetProgress("leccion-01")!.State);
        Assert.Equal(
            LessonState.InProgress,
            storePrevio.GetProgress("leccion-02")!.State);
    }

    [Fact]
    public void Restore_ArchivoCopiaIncompleta_PropagaErrorDeIntegridad()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var copia = _service.CreateBackup();

        var copiaIncompleta = Path.Combine(
            _db.DirectoryPath, "copia-incompleta.db");
        File.Copy(copia, copiaIncompleta, overwrite: true);
        using (var fs = new FileStream(
            copiaIncompleta,
            FileMode.Open,
            FileAccess.Write,
            FileShare.None))
        {
            fs.SetLength(1024);
        }

        var result = _service.Restore(copiaIncompleta);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e =>
            e.Contains("Integridad")
            || e.Contains("no pudo leer")
            || e.Contains("no existe")
            || e.Contains("versión de esquema")
            || e.Contains("lesson_progress"));

        Assert.Equal(
            LessonState.Mastered,
            _store.GetProgress("leccion-01")!.State);
    }

    [Fact]
    public void Restore_ArchivoVacío_PropagaErrorDeIntegridad()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var archivoVacio = Path.Combine(
            _db.DirectoryPath, "vacio.db");
        File.WriteAllBytes(archivoVacio, Array.Empty<byte>());

        var result = _service.Restore(archivoVacio);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);

        Assert.Equal(
            LessonState.Mastered,
            _store.GetProgress("leccion-01")!.State);
    }

    [Fact]
    public void Restore_ArchivoIncompatiblePorVersion_MantieneDatosActuales()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var copia = _service.CreateBackup();

        using (var conexion = _db.Db.CreateReadOnlyConnection())
        {
            conexion.Open();
            using var cmd = conexion.CreateCommand();
            cmd.CommandText = "PRAGMA user_version = 99;";
            cmd.ExecuteNonQuery();
        }

        var result = _service.Restore(copia);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e =>
            e.Contains("versión de esquema") || e.Contains("más nueva"));

        Assert.Equal(
            LessonState.Mastered,
            _store.GetProgress("leccion-01")!.State);
    }

    [Fact]
    public void Restore_CopiaValida_RecuperaElEstadoAnteriorYElRespaldoPrevioEsValido()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        GuardarProgreso("leccion-02", LessonState.InProgress);
        Assert.Equal(2, _store.GetAllProgress().Count);

        var copia = _service.CreateBackup();

        GuardarProgreso("leccion-01", LessonState.Completed);
        GuardarProgreso("leccion-03", LessonState.Completed);
        Assert.Equal(3, _store.GetAllProgress().Count);

        var result = _service.Restore(copia);

        Assert.True(result.Success, string.Join(", ", result.Errors));
        Assert.NotNull(result.SafetyBackupPath);
        Assert.True(File.Exists(result.SafetyBackupPath!));

        var validacionPrevia = _service.Validate(result.SafetyBackupPath!);
        Assert.True(validacionPrevia.IsValid);

        var store2 = new LearningStore(_db.Db);
        Assert.Equal(2, store2.GetAllProgress().Count);
        Assert.Equal(
            LessonState.Mastered,
            store2.GetProgress("leccion-01")!.State);
        Assert.Equal(
            LessonState.InProgress,
            store2.GetProgress("leccion-02")!.State);

        using var dbPrevia = new AppDb(result.SafetyBackupPath!);
        var storePrevio = new LearningStore(dbPrevia);
        Assert.Equal(3, storePrevio.GetAllProgress().Count);
        Assert.Equal(
            LessonState.Completed,
            storePrevio.GetProgress("leccion-01")!.State);
        Assert.Equal(
            LessonState.Completed,
            storePrevio.GetProgress("leccion-03")!.State);
    }

    [Fact]
    public void Restore_CopiaValida_ProduceArchivoRutaTi30ConVersionYConexionesVerificables()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var copia = _service.CreateBackup();

        var result = _service.Restore(copia);

        Assert.True(result.Success, string.Join(", ", result.Errors));

        using var connection = _db.Db.OpenConnection();
        var version = AppDb.GetSchemaVersion(connection);
        Assert.Equal(Migrations.CurrentVersion, version);

        var storeRecuperado = new LearningStore(_db.Db);
        Assert.Equal(
            LessonState.Mastered,
            storeRecuperado.GetProgress("leccion-01")!.State);
        Assert.False(
            File.Exists(_db.Db.DatabasePath + "-wal")
            || File.Exists(_db.Db.DatabasePath + "-shm"));
    }

    // Validación extra sobre la integridad real del archivo restaurado,
    // incluyendo la validación de la copia de seguridad automática del momento previo.
    [Fact]
    public void Restore_CopiaValida_ValidaRespaldoAutomáticoYEstadoRestaurado()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        GuardarProgreso("leccion-02", LessonState.InProgress);
        Assert.Equal(2, _store.GetAllProgress().Count);

        var copia = _service.CreateBackup();

        GuardarProgreso("leccion-01", LessonState.Completed);
        GuardarProgreso("leccion-03", LessonState.Completed);
        Assert.Equal(3, _store.GetAllProgress().Count);

        var result = _service.Restore(copia);

        Assert.True(result.Success, string.Join(", ", result.Errors));
        Assert.NotNull(result.SafetyBackupPath);

        // El respaldo automático se debe poder validar como una copia utilizable.
        var autobackupValidation = _service.Validate(result.SafetyBackupPath!);
        Assert.True(
            autobackupValidation.IsValid,
            string.Join(", ", autobackupValidation.Errors));

        // El estado restaurado vuelve al momento de la copia.
        var store2 = new LearningStore(_db.Db);
        Assert.Equal(2, store2.GetAllProgress().Count);
        Assert.Equal(
            LessonState.Mastered,
            store2.GetProgress("leccion-01")!.State);
        Assert.Equal(
            LessonState.InProgress,
            store2.GetProgress("leccion-02")!.State);

        // El respaldo automático contiene los datos posteriores a la copia.
        using var dbAutobackup = new AppDb(result.SafetyBackupPath!);
        var storeAutobackup = new LearningStore(dbAutobackup);
        Assert.Equal(3, storeAutobackup.GetAllProgress().Count);
        Assert.Equal(
            LessonState.Completed,
            storeAutobackup.GetProgress("leccion-01")!.State);
        Assert.Equal(
            LessonState.Completed,
            storeAutobackup.GetProgress("leccion-03")!.State);
    }

    // Validación final: después de una restauración exitosa con copia válida,
    // la base recuperada pasa las mismas comprobaciones que la validación de respaldos.
    [Fact]
    public void Restore_CopiaValida_LaBaseRestauradaPasaValidacionAutónoma()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var copia = _service.CreateBackup();

        GuardarProgreso("leccion-01", LessonState.Completed);
        GuardarProgreso("leccion-02", LessonState.InProgress);
        Assert.Equal(2, _store.GetAllProgress().Count);

        var result = _service.Restore(copia);

        Assert.True(result.Success, string.Join(", ", result.Errors));

        var validacionPostRestauracion = _service.Validate(_db.Db.DatabasePath);
        Assert.True(
            validacionPostRestauracion.IsValid,
            string.Join(", ", validacionPostRestauracion.Errors));
        Assert.Equal(
            Migrations.CurrentVersion,
            validacionPostRestauracion.SchemaVersion);
    }

    [Fact]
    public void Restore_DuranteIOException_NoDejaLaBaseInestableYUsaRespaldoPrevio()
    {
        GuardarProgreso("leccion-01", LessonState.Mastered);
        var copia = _service.CreateBackup();

        var tmpDir = _db.DirectoryPath;
        var blocker = new TempFileBlocker(
            Path.Combine(tmpDir, "backups"));

        try
        {
            var result = _service.Restore(copia);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);

            Assert.Equal(
                LessonState.Mastered,
                _store.GetProgress("leccion-01")!.State);
        }
        finally
        {
            blocker.Dispose();
        }
    }
}

// Bloqueador temporal para simular fallos de escritura del directorio de backups
// durante Restore. No se comparte entre pruebas: cada prueba crea y libera el suyo.
internal sealed class TempFileBlocker : IDisposable
{
    private readonly string _lockFile;

    public TempFileBlocker(string directoryPath)
    {
        _lockFile = Path.Combine(directoryPath, ".lock" + Guid.NewGuid().ToString("N"));
        var dir = Path.GetDirectoryName(_lockFile);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(_lockFile, "blocked");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_lockFile))
                File.Delete(_lockFile);
        }
        catch
        {
        }
    }

    public string LockFilePath => _lockFile;
}
