namespace RutaTI30.Domain;

/// <summary>Bloque del horario de estudio (editable por el usuario).</summary>
public sealed class ScheduleBlock
{
    public required int Id { get; set; }
    /// <summary>Minutos desde medianoche (inicio).</summary>
    public required int StartMinutes { get; set; }
    /// <summary>Minutos desde medianoche (fin). Debe ser mayor que el inicio.</summary>
    public required int EndMinutes { get; set; }
    public required ScheduleKind Kind { get; set; }
    public required string Label { get; set; }
    /// <summary>Minutos planificados del bloque (derivados, no se duplican datos: se recalculan al validar).</summary>
    public int PlannedMinutes => EndMinutes - StartMinutes;

    public static string FormatMinutes(int minutes) =>
        $"{minutes / 60:D2}:{minutes % 60:D2}";
}

/// <summary>Sesión de estudio registrada (tiempo planificado vs. tiempo efectivo verificado).</summary>
public sealed class StudySession
{
    public long Id { get; set; }
    public DateTime StartedUtc { get; set; }
    public DateTime? EndedUtc { get; set; }
    public int PlannedSeconds { get; set; }
    /// <summary>Tiempo efectivo verificado: solo períodos en curso, nunca pausas ni tiempo cerrado.</summary>
    public int EffectiveSeconds { get; set; }
    public ScheduleKind BlockKind { get; set; }
    /// <summary>Nota p. ej. "Interrumpida al cerrar la aplicación".</summary>
    public string Note { get; set; } = string.Empty;
}

/// <summary>Fila única (id = 1) con el estado persistido del temporizador para recuperación tras cierre inesperado.</summary>
public sealed class TimerStateRow
{
    public const int SingletonId = 1;
    public int Id { get; set; } = SingletonId;
    public TimerStatus Status { get; set; } = TimerStatus.Detenido;
    public long? SessionId { get; set; }
    public DateTime? StartedUtc { get; set; }
    /// <summary>Último instante desde el que corre el tiempo (inicio o reanudación).</summary>
    public DateTime? LastResumeUtc { get; set; }
    /// <summary>Segundos acumulados verificados de períodos anteriores (sin el período en curso).</summary>
    public int AccumulatedSeconds { get; set; }
    /// <summary>Último tick persistido (cada 30 s como máximo y en cada transición).</summary>
    public DateTime? PersistedTickUtc { get; set; }
    public string Note { get; set; } = string.Empty;
}
