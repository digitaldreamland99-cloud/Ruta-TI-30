using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Infrastructure;

/// <summary>Persistencia de los intentos de laboratorio (decisiones y puntajes).</summary>
public sealed class LabStore : ILabStore
{
    private readonly AppDb _db;

    public LabStore(AppDb db) => _db = db;

    public long AddAttempt(LabAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO lab_attempts (scenario_id, started_utc, finished_utc, score, decisions_json)
VALUES (@scenario, @started, @finished, @score, @decisions);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@scenario", attempt.ScenarioId);
        cmd.Parameters.AddWithValue("@started", DbFormat.Utc(attempt.StartedUtc));
        DbFormat.Add(cmd.Parameters, "@finished", attempt.FinishedUtc is null ? null : DbFormat.Utc(attempt.FinishedUtc.Value));
        cmd.Parameters.AddWithValue("@score", attempt.Score);
        cmd.Parameters.AddWithValue("@decisions", attempt.DecisionsJson);
        attempt.Id = Convert.ToInt64(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
        return attempt.Id;
    }

    public void UpdateAttempt(LabAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
UPDATE lab_attempts SET finished_utc = @finished, score = @score, decisions_json = @decisions
WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", attempt.Id);
        DbFormat.Add(cmd.Parameters, "@finished", attempt.FinishedUtc is null ? null : DbFormat.Utc(attempt.FinishedUtc.Value));
        cmd.Parameters.AddWithValue("@score", attempt.Score);
        cmd.Parameters.AddWithValue("@decisions", attempt.DecisionsJson);
        cmd.ExecuteNonQuery();
    }

    public IReadOnlyList<LabAttempt> GetAttempts(string? scenarioId = null)
    {
        var result = new List<LabAttempt>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = scenarioId is null
            ? @"SELECT id, scenario_id, started_utc, finished_utc, score, decisions_json FROM lab_attempts ORDER BY id DESC;"
            : @"SELECT id, scenario_id, started_utc, finished_utc, score, decisions_json FROM lab_attempts
                 WHERE scenario_id = @scenario ORDER BY id DESC;";
        if (scenarioId is not null)
            cmd.Parameters.AddWithValue("@scenario", scenarioId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new LabAttempt
            {
                Id = reader.GetInt64(0),
                ScenarioId = reader.GetString(1),
                StartedUtc = DbFormat.ParseUtc(reader.GetString(2)) ?? DateTime.MinValue,
                FinishedUtc = DbFormat.ParseUtc(reader.IsDBNull(3) ? null : reader.GetString(3)),
                Score = reader.GetDouble(4),
                DecisionsJson = reader.IsDBNull(5) ? "[]" : reader.GetString(5)
            });
        }
        return result;
    }
}
