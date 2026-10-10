using Microsoft.Data.Sqlite;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Infrastructure;

public class MigrationTests : IDisposable
{
    private readonly TempDb _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public void Initialize_AplicaLaUltimaVersionDeEsquema()
    {
        Assert.Equal(Migrations.CurrentVersion, _db.Db.GetSchemaVersion());
    }

    [Fact]
    public void Initialize_EsIdempotente()
    {
        _db.Db.Initialize();
        _db.Db.Initialize();

        Assert.Equal(Migrations.CurrentVersion, _db.Db.GetSchemaVersion());
    }

    [Fact]
    public void Initialize_CreaTodasLasTablasEsperadas()
    {
        var esperadas = new[]
        {
            "preferences", "lesson_progress", "evaluation_attempts", "study_sessions", "timer_state",
            "tickets", "ticket_history", "lab_attempts", "interview_attempts", "job_applications", "followups"
        };

        using var connection = _db.Db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table';";
        var existentes = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                existentes.Add(reader.GetString(0));
        }

        foreach (var tabla in esperadas)
            Assert.Contains(tabla, existentes);
    }

    [Fact]
    public void ConsultasParametrizadas_NoPermitenInyeccion()
    {
        using var connection = _db.Db.OpenConnection();
        using (var insert = connection.CreateCommand())
        {
            insert.CommandText = "INSERT INTO preferences (key, value) VALUES (@k, @v);";
            insert.Parameters.AddWithValue("@k", "usuario");
            insert.Parameters.AddWithValue("@v", "test' OR '1'='1");
            insert.ExecuteNonQuery();
        }

        using var query = connection.CreateCommand();
        query.CommandText = "SELECT value FROM preferences WHERE key = @k;";
        query.Parameters.AddWithValue("@k", "usuario");
        var value = query.ExecuteScalar() as string;

        Assert.Equal("test' OR '1'='1", value);
    }

    [Fact]
    public void Esquema_EnforcedClavePrimariaDuplicada()
    {
        using var connection = _db.Db.OpenConnection();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO preferences (key, value) VALUES (@k, @v);";
            cmd.Parameters.AddWithValue("@k", "dup");
            cmd.Parameters.AddWithValue("@v", "1");
            cmd.ExecuteNonQuery();
        }

        using var dup = connection.CreateCommand();
        dup.CommandText = "INSERT INTO preferences (key, value) VALUES (@k, @v);";
        dup.Parameters.AddWithValue("@k", "dup");
        dup.Parameters.AddWithValue("@v", "2");

        Assert.Throws<SqliteException>(() => dup.ExecuteNonQuery());
    }
}
