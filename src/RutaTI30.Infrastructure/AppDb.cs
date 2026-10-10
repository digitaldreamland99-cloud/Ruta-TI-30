using Microsoft.Data.Sqlite;

namespace RutaTI30.Infrastructure;

/// <summary>
/// Acceso a la base de datos SQLite local: creación de conexiones, inicialización y migraciones.
/// Todas las consultas son parametrizadas; cada migración se aplica en transacción.
/// </summary>
public sealed class AppDb : IDisposable
{
    private readonly string _connectionString;

    public string DatabasePath { get; }

    public AppDb(string databasePath)
    {
        DatabasePath = databasePath ?? throw new ArgumentNullException(nameof(databasePath));
        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    /// <summary>Abre una conexión con claves foráneas activas. El llamador debe disponerla.</summary>
    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys = ON;";
        cmd.ExecuteNonQuery();
        return connection;
    }

    /// <summary>Inicializa la base de datos y aplica las migraciones pendientes en transacción.</summary>
    public void Initialize()
    {
        using var connection = OpenConnection();
        using (var wal = connection.CreateCommand())
        {
            wal.CommandText = "PRAGMA journal_mode = WAL;";
            wal.ExecuteNonQuery();
        }

        int current = GetSchemaVersion(connection);
        foreach (var (version, sql) in Migrations.All.Where(m => m.Version > current).OrderBy(m => m.Version))
        {
            using var transaction = connection.BeginTransaction();
            try
            {
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                using var setVersion = connection.CreateCommand();
                setVersion.Transaction = transaction;
                setVersion.CommandText = "PRAGMA user_version = @version;";
                setVersion.Parameters.AddWithValue("@version", version);
                setVersion.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>Versión actual del esquema (PRAGMA user_version).</summary>
    public static int GetSchemaVersion(SqliteConnection connection)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "PRAGMA user_version;";
        return Convert.ToInt32(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    public int GetSchemaVersion()
    {
        using var connection = OpenConnection();
        return GetSchemaVersion(connection);
    }

    public void Dispose()
    {
        // No hay conexiones persistentes: se abren por operación y se cierran con 'using'.
        SqliteConnection.ClearAllPools();
    }
}
