namespace RutaTI30.Infrastructure;

/// <summary>Migraciones versionadas del esquema. Cada migración corre en transacción;
/// el progreso del usuario nunca se borra al actualizar (solo se añaden tablas/columnas).</summary>
public static class Migrations
{
    /// <summary>Versión actual del esquema que espera la aplicación.</summary>
    public const int CurrentVersion = 1;

    public static readonly IReadOnlyList<(int Version, string Sql)> All = new List<(int, string)>
    {
        (1, InitialSchema)
    };

    private const string InitialSchema = @"
CREATE TABLE IF NOT EXISTS preferences (
    key   TEXT PRIMARY KEY NOT NULL,
    value TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS lesson_progress (
    lesson_id    TEXT PRIMARY KEY NOT NULL,
    state        INTEGER NOT NULL DEFAULT 0,
    started_utc  TEXT NULL,
    completed_utc TEXT NULL,
    mastered_utc TEXT NULL,
    last_score   REAL NULL
);

CREATE TABLE IF NOT EXISTS evaluation_attempts (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    lesson_id     TEXT NOT NULL,
    attempted_utc TEXT NOT NULL,
    score         REAL NOT NULL,
    passed        INTEGER NOT NULL,
    details_json  TEXT NOT NULL DEFAULT '{}'
);
CREATE INDEX IF NOT EXISTS ix_attempts_lesson ON evaluation_attempts(lesson_id);

CREATE TABLE IF NOT EXISTS study_sessions (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    started_utc      TEXT NOT NULL,
    ended_utc        TEXT NULL,
    planned_seconds  INTEGER NOT NULL,
    effective_seconds INTEGER NOT NULL DEFAULT 0,
    block_kind       INTEGER NOT NULL,
    note             TEXT NOT NULL DEFAULT ''
);
CREATE INDEX IF NOT EXISTS ix_sessions_started ON study_sessions(started_utc);

CREATE TABLE IF NOT EXISTS timer_state (
    id                 INTEGER PRIMARY KEY CHECK (id = 1),
    status             INTEGER NOT NULL,
    session_id         INTEGER NULL,
    started_utc        TEXT NULL,
    last_resume_utc    TEXT NULL,
    accumulated_seconds INTEGER NOT NULL DEFAULT 0,
    persisted_tick_utc TEXT NULL,
    note               TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS tickets (
    id                   TEXT PRIMARY KEY NOT NULL,
    created_utc          TEXT NOT NULL,
    updated_utc          TEXT NOT NULL,
    reporter             TEXT NOT NULL,
    department           TEXT NOT NULL DEFAULT '',
    description          TEXT NOT NULL,
    category             INTEGER NOT NULL DEFAULT 0,
    impact               INTEGER NOT NULL DEFAULT 1,
    urgency              INTEGER NOT NULL DEFAULT 1,
    priority             INTEGER NOT NULL DEFAULT 3,
    state                INTEGER NOT NULL DEFAULT 0,
    resolution           TEXT NOT NULL DEFAULT '',
    escalated            INTEGER NOT NULL DEFAULT 0,
    target_response_hours INTEGER NOT NULL DEFAULT 8
);
CREATE INDEX IF NOT EXISTS ix_tickets_state ON tickets(state);

CREATE TABLE IF NOT EXISTS ticket_history (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    ticket_id  TEXT NOT NULL REFERENCES tickets(id) ON DELETE CASCADE,
    at_utc     TEXT NOT NULL,
    action     TEXT NOT NULL,
    note       TEXT NOT NULL DEFAULT '',
    from_state INTEGER NULL,
    to_state   INTEGER NULL
);
CREATE INDEX IF NOT EXISTS ix_history_ticket ON ticket_history(ticket_id);

CREATE TABLE IF NOT EXISTS lab_attempts (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    scenario_id    TEXT NOT NULL,
    started_utc    TEXT NOT NULL,
    finished_utc   TEXT NULL,
    score          REAL NOT NULL DEFAULT 0,
    decisions_json TEXT NOT NULL DEFAULT '[]'
);
CREATE INDEX IF NOT EXISTS ix_lab_scenario ON lab_attempts(scenario_id);

CREATE TABLE IF NOT EXISTS interview_attempts (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    question_id   TEXT NOT NULL,
    attempted_utc TEXT NOT NULL,
    answer_text   TEXT NOT NULL DEFAULT '',
    self_rating   INTEGER NULL,
    notes         TEXT NOT NULL DEFAULT ''
);
CREATE INDEX IF NOT EXISTS ix_interview_question ON interview_attempts(question_id);

CREATE TABLE IF NOT EXISTS job_applications (
    id                INTEGER PRIMARY KEY AUTOINCREMENT,
    company           TEXT NOT NULL,
    role              TEXT NOT NULL,
    location          TEXT NOT NULL DEFAULT '',
    discovered_utc    TEXT NULL,
    applied_utc       TEXT NULL,
    url               TEXT NOT NULL DEFAULT '',
    salary            TEXT NOT NULL DEFAULT '',
    state             INTEGER NOT NULL DEFAULT 0,
    contact           TEXT NOT NULL DEFAULT '',
    interview_utc     TEXT NULL,
    next_followup_utc TEXT NULL,
    notes             TEXT NOT NULL DEFAULT '',
    result            TEXT NOT NULL DEFAULT ''
);
CREATE INDEX IF NOT EXISTS ix_jobs_state ON job_applications(state);

CREATE TABLE IF NOT EXISTS followups (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    application_id INTEGER NOT NULL REFERENCES job_applications(id) ON DELETE CASCADE,
    at_utc         TEXT NOT NULL,
    note           TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_followups_app ON followups(application_id);
";
}
