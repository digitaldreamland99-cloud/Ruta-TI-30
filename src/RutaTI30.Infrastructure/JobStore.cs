using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Infrastructure;

/// <summary>Persistencia del banco de respuestas de entrevistas y del registro de postulaciones.</summary>
public sealed class JobStore : IJobStore
{
    private readonly AppDb _db;

    public JobStore(AppDb db) => _db = db;

    public long AddApplication(JobApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO job_applications (company, role, location, discovered_utc, applied_utc, url, salary,
    state, contact, interview_utc, next_followup_utc, notes, result)
VALUES (@company, @role, @location, @discovered, @applied, @url, @salary,
    @state, @contact, @interview, @followup, @notes, @result);
SELECT last_insert_rowid();";
        AddApplicationParameters(cmd, application);
        application.Id = Convert.ToInt64(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
        return application.Id;
    }

    public void UpdateApplication(JobApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
UPDATE job_applications SET
    company = @company, role = @role, location = @location,
    discovered_utc = @discovered, applied_utc = @applied, url = @url, salary = @salary,
    state = @state, contact = @contact, interview_utc = @interview,
    next_followup_utc = @followup, notes = @notes, result = @result
WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", application.Id);
        AddApplicationParameters(cmd, application);
        cmd.ExecuteNonQuery();
    }

    public void DeleteApplication(long id)
    {
        using var connection = _db.OpenConnection();
        using var transaction = connection.BeginTransaction();

        using (var delFollowUps = connection.CreateCommand())
        {
            delFollowUps.Transaction = transaction;
            delFollowUps.CommandText = "DELETE FROM followups WHERE application_id = @id;";
            delFollowUps.Parameters.AddWithValue("@id", id);
            delFollowUps.ExecuteNonQuery();
        }

        using (var delApp = connection.CreateCommand())
        {
            delApp.Transaction = transaction;
            delApp.CommandText = "DELETE FROM job_applications WHERE id = @id;";
            delApp.Parameters.AddWithValue("@id", id);
            delApp.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public JobApplication? GetApplication(long id)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = SelectSql + " WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Read(reader) : null;
    }

    public IReadOnlyList<JobApplication> ListApplications()
    {
        var result = new List<JobApplication>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = SelectSql + " ORDER BY id DESC;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(Read(reader));
        return result;
    }

    public void AddFollowUp(FollowUp followUp)
    {
        ArgumentNullException.ThrowIfNull(followUp);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO followups (application_id, at_utc, note) VALUES (@app, @at, @note);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@app", followUp.ApplicationId);
        cmd.Parameters.AddWithValue("@at", DbFormat.Utc(followUp.AtUtc));
        cmd.Parameters.AddWithValue("@note", followUp.Note);
        followUp.Id = Convert.ToInt64(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    public IReadOnlyList<FollowUp> GetFollowUps(long applicationId)
    {
        var result = new List<FollowUp>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, application_id, at_utc, note FROM followups WHERE application_id = @app ORDER BY at_utc DESC;";
        cmd.Parameters.AddWithValue("@app", applicationId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FollowUp
            {
                Id = reader.GetInt64(0),
                ApplicationId = reader.GetInt64(1),
                AtUtc = DbFormat.ParseUtc(reader.GetString(2)) ?? DateTime.MinValue,
                Note = reader.GetString(3)
            });
        }
        return result;
    }

    public void AddInterviewAttempt(InterviewAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO interview_attempts (question_id, attempted_utc, answer_text, self_rating, notes)
VALUES (@question, @at, @answer, @rating, @notes);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@question", attempt.QuestionId);
        cmd.Parameters.AddWithValue("@at", DbFormat.Utc(attempt.AttemptedUtc));
        cmd.Parameters.AddWithValue("@answer", attempt.AnswerText);
        DbFormat.Add(cmd.Parameters, "@rating", attempt.SelfRating);
        cmd.Parameters.AddWithValue("@notes", attempt.Notes);
        attempt.Id = Convert.ToInt64(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    public IReadOnlyList<InterviewAttempt> GetInterviewAttempts(string? questionId = null)
    {
        var result = new List<InterviewAttempt>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = questionId is null
            ? @"SELECT id, question_id, attempted_utc, answer_text, self_rating, notes
FROM interview_attempts ORDER BY id DESC;"
            : @"SELECT id, question_id, attempted_utc, answer_text, self_rating, notes
FROM interview_attempts WHERE question_id = @question ORDER BY id DESC;";
        if (questionId is not null)
            cmd.Parameters.AddWithValue("@question", questionId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new InterviewAttempt
            {
                Id = reader.GetInt64(0),
                QuestionId = reader.GetString(1),
                AttemptedUtc = DbFormat.ParseUtc(reader.GetString(2)) ?? DateTime.MinValue,
                AnswerText = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                SelfRating = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Notes = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            });
        }
        return result;
    }

    private const string SelectSql = @"SELECT id, company, role, location, discovered_utc, applied_utc, url, salary,
    state, contact, interview_utc, next_followup_utc, notes, result
FROM job_applications";

    private static void AddApplicationParameters(Microsoft.Data.Sqlite.SqliteCommand cmd, JobApplication app)
    {
        cmd.Parameters.AddWithValue("@company", app.Company);
        cmd.Parameters.AddWithValue("@role", app.Role);
        cmd.Parameters.AddWithValue("@location", app.Location);
        DbFormat.Add(cmd.Parameters, "@discovered", app.DiscoveredUtc is null ? null : DbFormat.Utc(app.DiscoveredUtc.Value));
        DbFormat.Add(cmd.Parameters, "@applied", app.AppliedUtc is null ? null : DbFormat.Utc(app.AppliedUtc.Value));
        cmd.Parameters.AddWithValue("@url", app.Url);
        cmd.Parameters.AddWithValue("@salary", app.Salary);
        cmd.Parameters.AddWithValue("@state", (int)app.State);
        cmd.Parameters.AddWithValue("@contact", app.Contact);
        DbFormat.Add(cmd.Parameters, "@interview", app.InterviewUtc is null ? null : DbFormat.Utc(app.InterviewUtc.Value));
        DbFormat.Add(cmd.Parameters, "@followup", app.NextFollowUpUtc is null ? null : DbFormat.Utc(app.NextFollowUpUtc.Value));
        cmd.Parameters.AddWithValue("@notes", app.Notes);
        cmd.Parameters.AddWithValue("@result", app.Result);
    }

    private static JobApplication Read(Microsoft.Data.Sqlite.SqliteDataReader reader) => new()
    {
        Id = reader.GetInt64(0),
        Company = reader.GetString(1),
        Role = reader.GetString(2),
        Location = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
        DiscoveredUtc = DbFormat.ParseUtc(reader.IsDBNull(4) ? null : reader.GetString(4)),
        AppliedUtc = DbFormat.ParseUtc(reader.IsDBNull(5) ? null : reader.GetString(5)),
        Url = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
        Salary = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
        State = (JobState)reader.GetInt32(8),
        Contact = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
        InterviewUtc = DbFormat.ParseUtc(reader.IsDBNull(10) ? null : reader.GetString(10)),
        NextFollowUpUtc = DbFormat.ParseUtc(reader.IsDBNull(11) ? null : reader.GetString(11)),
        Notes = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
        Result = reader.IsDBNull(13) ? string.Empty : reader.GetString(13)
    };
}
