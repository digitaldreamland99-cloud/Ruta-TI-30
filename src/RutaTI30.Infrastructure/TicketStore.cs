using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Infrastructure;

/// <summary>Persistencia de tickets de Help Desk con historial (alta transaccional ticket + historial).</summary>
public sealed class TicketStore : ITicketStore
{
    private readonly AppDb _db;

    public TicketStore(AppDb db) => _db = db;

    public void AddWithHistory(Ticket ticket, TicketHistoryEntry creationEntry)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ArgumentNullException.ThrowIfNull(creationEntry);

        using var connection = _db.OpenConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = @"
INSERT INTO tickets (id, created_utc, updated_utc, reporter, department, description, category,
    impact, urgency, priority, state, resolution, escalated, target_response_hours)
VALUES (@id, @created, @updated, @reporter, @department, @description, @category,
    @impact, @urgency, @priority, @state, @resolution, @escalated, @target);";
                AddTicketParameters(cmd, ticket);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = @"
INSERT INTO ticket_history (ticket_id, at_utc, action, note, from_state, to_state)
VALUES (@ticket_id, @at, @action, @note, @from_state, @to_state);";
                AddHistoryParameters(cmd, creationEntry);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Update(Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
UPDATE tickets SET
    updated_utc = @updated,
    reporter = @reporter,
    department = @department,
    description = @description,
    category = @category,
    impact = @impact,
    urgency = @urgency,
    priority = @priority,
    state = @state,
    resolution = @resolution,
    escalated = @escalated,
    target_response_hours = @target
WHERE id = @id;";
        AddTicketParameters(cmd, ticket);
        cmd.ExecuteNonQuery();
    }

    public void AddHistory(TicketHistoryEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO ticket_history (ticket_id, at_utc, action, note, from_state, to_state)
VALUES (@ticket_id, @at, @action, @note, @from_state, @to_state);";
        AddHistoryParameters(cmd, entry);
        cmd.ExecuteNonQuery();
    }

    public Ticket? Get(string id)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = SelectTicketsSql + " WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Read(reader) : null;
    }

    public IReadOnlyList<Ticket> List(TicketState? stateFilter = null, string? search = null)
    {
        var clauses = new List<string>();
        var result = new List<Ticket>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();

        if (stateFilter is not null)
            clauses.Add("state = @state");
        if (!string.IsNullOrWhiteSpace(search))
            clauses.Add("(id LIKE @search OR reporter LIKE @search OR description LIKE @search OR department LIKE @search)");

        cmd.CommandText = SelectTicketsSql +
            (clauses.Count > 0 ? " WHERE " + string.Join(" AND ", clauses) : "") +
            " ORDER BY created_utc DESC;";

        if (stateFilter is not null)
            cmd.Parameters.AddWithValue("@state", (int)stateFilter.Value);
        if (!string.IsNullOrWhiteSpace(search))
            cmd.Parameters.AddWithValue("@search", $"%{search.Trim()}%");

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(Read(reader));
        return result;
    }

    public IReadOnlyList<TicketHistoryEntry> GetHistory(string ticketId)
    {
        var result = new List<TicketHistoryEntry>();
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT id, ticket_id, at_utc, action, note, from_state, to_state
FROM ticket_history WHERE ticket_id = @ticket_id ORDER BY id;";
        cmd.Parameters.AddWithValue("@ticket_id", ticketId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new TicketHistoryEntry
            {
                Id = reader.GetInt64(0),
                TicketId = reader.GetString(1),
                AtUtc = DbFormat.ParseUtc(reader.GetString(2)) ?? DateTime.MinValue,
                Action = reader.GetString(3),
                Note = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                FromState = reader.IsDBNull(5) ? null : (TicketState)reader.GetInt32(5),
                ToState = reader.IsDBNull(6) ? null : (TicketState)reader.GetInt32(6)
            });
        }
        return result;
    }

    public int CountByState(TicketState state)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM tickets WHERE state = @state;";
        cmd.Parameters.AddWithValue("@state", (int)state);
        return Convert.ToInt32(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    public int CountAll()
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM tickets;";
        return Convert.ToInt32(cmd.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private const string SelectTicketsSql = @"SELECT id, created_utc, updated_utc, reporter, department, description,
    category, impact, urgency, priority, state, resolution, escalated, target_response_hours
FROM tickets";

    private static void AddTicketParameters(Microsoft.Data.Sqlite.SqliteCommand cmd, Ticket ticket)
    {
        cmd.Parameters.AddWithValue("@id", ticket.Id);
        cmd.Parameters.AddWithValue("@created", DbFormat.Utc(ticket.CreatedUtc));
        cmd.Parameters.AddWithValue("@updated", DbFormat.Utc(ticket.UpdatedUtc));
        cmd.Parameters.AddWithValue("@reporter", ticket.Reporter);
        cmd.Parameters.AddWithValue("@department", ticket.Department);
        cmd.Parameters.AddWithValue("@description", ticket.Description);
        cmd.Parameters.AddWithValue("@category", (int)ticket.Category);
        cmd.Parameters.AddWithValue("@impact", (int)ticket.Impact);
        cmd.Parameters.AddWithValue("@urgency", (int)ticket.Urgency);
        cmd.Parameters.AddWithValue("@priority", (int)ticket.Priority);
        cmd.Parameters.AddWithValue("@state", (int)ticket.State);
        cmd.Parameters.AddWithValue("@resolution", ticket.Resolution);
        cmd.Parameters.AddWithValue("@escalated", DbFormat.BoolToInt(ticket.Escalated));
        cmd.Parameters.AddWithValue("@target", ticket.TargetResponseHours);
    }

    private static void AddHistoryParameters(Microsoft.Data.Sqlite.SqliteCommand cmd, TicketHistoryEntry entry)
    {
        cmd.Parameters.AddWithValue("@ticket_id", entry.TicketId);
        cmd.Parameters.AddWithValue("@at", DbFormat.Utc(entry.AtUtc));
        cmd.Parameters.AddWithValue("@action", entry.Action);
        cmd.Parameters.AddWithValue("@note", entry.Note);
        DbFormat.Add(cmd.Parameters, "@from_state", entry.FromState is null ? null : (int)entry.FromState.Value);
        DbFormat.Add(cmd.Parameters, "@to_state", entry.ToState is null ? null : (int)entry.ToState.Value);
    }

    private static Ticket Read(Microsoft.Data.Sqlite.SqliteDataReader reader) => new()
    {
        Id = reader.GetString(0),
        CreatedUtc = DbFormat.ParseUtc(reader.GetString(1)) ?? DateTime.MinValue,
        UpdatedUtc = DbFormat.ParseUtc(reader.GetString(2)) ?? DateTime.MinValue,
        Reporter = reader.GetString(3),
        Department = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
        Description = reader.GetString(5),
        Category = (TicketCategory)reader.GetInt32(6),
        Impact = (ImpactLevel)reader.GetInt32(7),
        Urgency = (UrgencyLevel)reader.GetInt32(8),
        Priority = (PriorityLevel)reader.GetInt32(9),
        State = (TicketState)reader.GetInt32(10),
        Resolution = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
        Escalated = DbFormat.IntToBool(reader.GetInt32(12)),
        TargetResponseHours = reader.GetInt32(13)
    };
}
