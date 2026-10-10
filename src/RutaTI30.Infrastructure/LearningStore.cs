using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Infrastructure;

/// <summary>Persistencia del progreso de lecciones y del historial de evaluaciones.</summary>
public sealed class LearningStore : ILearningStore
{
    private readonly AppDb _db;

    public LearningStore(AppDb db) => _db = db;

    public IReadOnlyList<LessonProgress> GetAllProgress()
    {
        var result = new List<LessonProgress>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT lesson_id, state, started_utc, completed_utc, mastered_utc, last_score
FROM lesson_progress ORDER BY lesson_id;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new LessonProgress
            {
                LessonId = reader.GetString(0),
                State = (LessonState)reader.GetInt32(1),
                StartedUtc = DbFormat.ParseUtc(reader.IsDBNull(2) ? null : reader.GetString(2)),
                CompletedUtc = DbFormat.ParseUtc(reader.IsDBNull(3) ? null : reader.GetString(3)),
                MasteredUtc = DbFormat.ParseUtc(reader.IsDBNull(4) ? null : reader.GetString(4)),
                LastScore = reader.IsDBNull(5) ? null : reader.GetDouble(5)
            });
        }
        return result;
    }

    public LessonProgress? GetProgress(string lessonId)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT lesson_id, state, started_utc, completed_utc, mastered_utc, last_score
FROM lesson_progress WHERE lesson_id = @id;";
        cmd.Parameters.AddWithValue("@id", lessonId);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new LessonProgress
        {
            LessonId = reader.GetString(0),
            State = (LessonState)reader.GetInt32(1),
            StartedUtc = DbFormat.ParseUtc(reader.IsDBNull(2) ? null : reader.GetString(2)),
            CompletedUtc = DbFormat.ParseUtc(reader.IsDBNull(3) ? null : reader.GetString(3)),
            MasteredUtc = DbFormat.ParseUtc(reader.IsDBNull(4) ? null : reader.GetString(4)),
            LastScore = reader.IsDBNull(5) ? null : reader.GetDouble(5)
        };
    }

    public void SaveProgress(LessonProgress progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO lesson_progress (lesson_id, state, started_utc, completed_utc, mastered_utc, last_score)
VALUES (@lesson_id, @state, @started, @completed, @mastered, @score)
ON CONFLICT(lesson_id) DO UPDATE SET
    state = excluded.state,
    started_utc = excluded.started_utc,
    completed_utc = excluded.completed_utc,
    mastered_utc = excluded.mastered_utc,
    last_score = excluded.last_score;";
        cmd.Parameters.AddWithValue("@lesson_id", progress.LessonId);
        cmd.Parameters.AddWithValue("@state", (int)progress.State);
        DbFormat.Add(cmd.Parameters, "@started", progress.StartedUtc is null ? null : DbFormat.Utc(progress.StartedUtc.Value));
        DbFormat.Add(cmd.Parameters, "@completed", progress.CompletedUtc is null ? null : DbFormat.Utc(progress.CompletedUtc.Value));
        DbFormat.Add(cmd.Parameters, "@mastered", progress.MasteredUtc is null ? null : DbFormat.Utc(progress.MasteredUtc.Value));
        DbFormat.Add(cmd.Parameters, "@score", progress.LastScore);
        cmd.ExecuteNonQuery();
    }

    public IReadOnlyList<EvaluationAttempt> GetAttempts(string? lessonId = null)
    {
        var result = new List<EvaluationAttempt>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = lessonId is null
            ? @"SELECT id, lesson_id, attempted_utc, score, passed, details_json FROM evaluation_attempts ORDER BY id;"
            : @"SELECT id, lesson_id, attempted_utc, score, passed, details_json FROM evaluation_attempts
                 WHERE lesson_id = @lesson_id ORDER BY id;";
        if (lessonId is not null)
            cmd.Parameters.AddWithValue("@lesson_id", lessonId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new EvaluationAttempt
            {
                Id = reader.GetInt64(0),
                LessonId = reader.GetString(1),
                AttemptedUtc = DbFormat.ParseUtc(reader.GetString(2)) ?? DateTime.MinValue,
                Score = reader.GetDouble(3),
                Passed = DbFormat.IntToBool(reader.GetInt32(4)),
                DetailsJson = reader.GetString(5)
            });
        }
        return result;
    }

    public void AddAttempt(EvaluationAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO evaluation_attempts (lesson_id, attempted_utc, score, passed, details_json)
VALUES (@lesson_id, @attempted, @score, @passed, @details);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@lesson_id", attempt.LessonId);
        cmd.Parameters.AddWithValue("@attempted", DbFormat.Utc(attempt.AttemptedUtc));
        cmd.Parameters.AddWithValue("@score", attempt.Score);
        cmd.Parameters.AddWithValue("@passed", DbFormat.BoolToInt(attempt.Passed));
        cmd.Parameters.AddWithValue("@details", attempt.DetailsJson);
        attempt.Id = Convert.ToInt64(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }
}
