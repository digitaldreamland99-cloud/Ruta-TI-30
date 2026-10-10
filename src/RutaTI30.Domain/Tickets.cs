namespace RutaTI30.Domain;

/// <summary>Ticket de ayuda ficticio (datos educativos, no reales).</summary>
public sealed class Ticket
{
    public required string Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    /// <summary>Usuario ficticio afectado.</summary>
    public required string Reporter { get; set; }
    public string Department { get; set; } = string.Empty;
    public required string Description { get; set; }
    public TicketCategory Category { get; set; }
    public ImpactLevel Impact { get; set; }
    public UrgencyLevel Urgency { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.P3;
    public TicketState State { get; set; } = TicketState.Nuevo;
    public string Resolution { get; set; } = string.Empty;
    public bool Escalated { get; set; }
    /// <summary>Objetivo de atención en horas (ejemplo didáctico configurable, no estándar universal).</summary>
    public int TargetResponseHours { get; set; } = 8;
}

/// <summary>Entrada del historial de un ticket: toda acción queda registrada.</summary>
public sealed class TicketHistoryEntry
{
    public long Id { get; set; }
    public required string TicketId { get; set; }
    public DateTime AtUtc { get; set; }
    public required string Action { get; set; }
    public string Note { get; set; } = string.Empty;
    public TicketState? FromState { get; set; }
    public TicketState? ToState { get; set; }
}

/// <summary>Reglas puras del simulador: prioridad y transiciones de estado.</summary>
public static class TicketRules
{
    /// <summary>Prioridad calculada a partir de impacto × urgencia.</summary>
    public static PriorityLevel CalculatePriority(ImpactLevel impact, UrgencyLevel urgency) =>
        (impact, urgency) switch
        {
            (ImpactLevel.Alto, UrgencyLevel.Alta) => PriorityLevel.P1,
            (ImpactLevel.Alto, UrgencyLevel.Media) => PriorityLevel.P2,
            (ImpactLevel.Medio, UrgencyLevel.Alta) => PriorityLevel.P2,
            (ImpactLevel.Alto, UrgencyLevel.Baja) => PriorityLevel.P3,
            (ImpactLevel.Medio, UrgencyLevel.Media) => PriorityLevel.P3,
            (ImpactLevel.Bajo, UrgencyLevel.Alta) => PriorityLevel.P3,
            _ => PriorityLevel.P4
        };

    /// <summary>Transiciones válidas. Toda otra transición debe rechazarse.</summary>
    public static bool CanTransition(TicketState from, TicketState to)
    {
        if (from == to) return false;
        return from switch
        {
            TicketState.Nuevo => to is TicketState.EnAtencion or TicketState.Escalado or TicketState.Cerrado,
            TicketState.EnAtencion => to is TicketState.EnEsperaUsuario or TicketState.Escalado or TicketState.Resuelto,
            TicketState.EnEsperaUsuario => to is TicketState.EnAtencion or TicketState.Escalado or TicketState.Resuelto,
            TicketState.Escalado => to is TicketState.EnAtencion or TicketState.Resuelto,
            TicketState.Resuelto => to is TicketState.Cerrado or TicketState.EnAtencion,
            TicketState.Cerrado => false,
            _ => false
        };
    }

    /// <summary>Valida los datos de creación/edición de un ticket.</summary>
    public static IReadOnlyList<string> Validate(string reporter, string description)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(reporter))
            errors.Add("El usuario afectado es obligatorio.");
        else if (reporter.Trim().Length > 120)
            errors.Add("El usuario afectado no puede exceder 120 caracteres.");
        if (string.IsNullOrWhiteSpace(description))
            errors.Add("La descripción es obligatoria.");
        else if (description.Trim().Length < 10)
            errors.Add("La descripción debe tener al menos 10 caracteres.");
        return errors;
    }
}
