using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Infrastructure;

/// <summary>Persistencia de sesiones de estudio y del estado recuperable del temporizador.</summary>
public sealed class SessionStore : ISessionStore
{
    private readonly AppDb _db;

    public SessionStore(AppDb db) => _db = db;

    public void SaveSession(StudySession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        if (session.Id == 0)
        {
            cmd.CommandText = @"
INSERT INTO study_sessions (started_utc, ended_utc, planned_seconds, effective_seconds, block_kind, note)
VALUES (@started, @ended, @planned, @effective, @block, @note);
SELECT last_insert_rowid();";
        }
        else
        {
            cmd.CommandText = @"
UPDATE study_sessions SET
    started_utc = @started,
    ended_utc = @ended,
    planned_seconds = @planned,
    effective_seconds = @effective,
    block_kind = @block,
    note = @note
WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", session.Id);
        }

        cmd.Parameters.AddWithValue("@started", DbFormat.Utc(session.StartedUtc));
        DbFormat.Add(cmd.Parameters, "@ended", session.EndedUtc is null ? null : DbFormat.Utc(session.EndedUtc.Value));
        cmd.Parameters.AddWithValue("@planned", session.PlannedSeconds);
        cmd.Parameters.AddWithValue("@effective", session.EffectiveSeconds);
        cmd.Parameters.AddWithValue("@block", (int)session.BlockKind);
        cmd.Parameters.AddWithValue("@note", session.Note);

        var result = cmd.ExecuteScalar();
        if (result is not null)
            session.Id = Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture);
    }

    public StudySession? GetSession(long id)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT id, started_utc, ended_utc, planned_seconds, effective_seconds, block_kind, note
FROM study_sessions WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Read(reader) : null;
    }

    public IReadOnlyList<StudySession> GetSessions(DateTime? fromUtc = null)
    {
        var result = new List<StudySession>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = fromUtc is null
            ? @"SELECT id, started_utc, ended_utc, planned_seconds, effective_seconds, block_kind, note
FROM study_sessions ORDER BY started_utc DESC;"
            : @"SELECT id, started_utc, ended_utc, planned_seconds, effective_seconds, block_kind, note
FROM study_sessions WHERE started_utc >= @from ORDER BY started_utc DESC;";
        if (fromUtc is not null)
            cmd.Parameters.AddWithValue("@from", DbFormat.Utc(fromUtc.Value));

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(Read(reader));
        return result;
    }

    public int GetTotalEffectiveSeconds()
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COALESCE(SUM(effective_seconds), 0) FROM study_sessions;";
        return Convert.ToInt32(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    public TimerStateRow GetTimerState()
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT id, status, session_id, started_utc, last_resume_utc,
       accumulated_seconds, persisted_tick_utc, note
FROM timer_state WHERE id = 1;";
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return new TimerStateRow();

        return new TimerStateRow
        {
            Id = reader.GetInt32(0),
            Status = (TimerStatus)reader.GetInt32(1),
            SessionId = reader.IsDBNull(2) ? null : reader.GetInt64(2),
            StartedUtc = DbFormat.ParseUtc(reader.IsDBNull(3) ? null : reader.GetString(3)),
            LastResumeUtc = DbFormat.ParseUtc(reader.IsDBNull(4) ? null : reader.GetString(4)),
            AccumulatedSeconds = reader.GetInt32(5),
            PersistedTickUtc = DbFormat.ParseUtc(reader.IsDBNull(6) ? null : reader.GetString(6)),
            Note = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
        };
    }

    public void SaveTimerState(TimerStateRow state)
    {
        ArgumentNullException.ThrowIfNull(state);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO timer_state (id, status, session_id, started_utc, last_resume_utc, accumulated_seconds, persisted_tick_utc, note)
VALUES (1, @status, @session, @started, @resume, @accumulated, @tick, @note)
ON CONFLICT(id) DO UPDATE SET
    status = excluded.status,
    session_id = excluded.session_id,
    started_utc = excluded.started_utc,
    last_resume_utc = excluded.last_resume_utc,
    accumulated_seconds = excluded.accumulated_seconds,
    persisted_tick_utc = excluded.persisted_tick_utc,
    note = excluded.note;";
        cmd.Parameters.AddWithValue("@status", (int)state.Status);
        DbFormat.Add(cmd.Parameters, "@session", state.SessionId);
        DbFormat.Add(cmd.Parameters, "@started", state.StartedUtc is null ? null : DbFormat.Utc(state.StartedUtc.Value));
        DbFormat.Add(cmd.Parameters, "@resume", state.LastResumeUtc is null ? null : DbFormat.Utc(state.LastResumeUtc.Value));
        cmd.Parameters.AddWithValue("@accumulated", state.AccumulatedSeconds);
        DbFormat.Add(cmd.Parameters, "@tick", state.PersistedTickUtc is null ? null : DbFormat.Utc(state.PersistedTickUtc.Value));
        cmd.Parameters.AddWithValue("@note", state.Note);
        cmd.ExecuteNonQuery();
    }

    private static StudySession Read(Microsoft.Data.Sqlite.SqliteDataReader reader) => new()
    {
        Id = reader.GetInt64(0),
        StartedUtc = DbFormat.ParseUtc(reader.GetString(1)) ?? DateTime.MinValue,
        EndedUtc = DbFormat.ParseUtc(reader.IsDBNull(2) ? null : reader.GetString(2)),
        PlannedSeconds = reader.GetInt32(3),
        EffectiveSeconds = reader.GetInt32(4),
        BlockKind = (ScheduleKind)reader.GetInt32(5),
        Note = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
    };
}
