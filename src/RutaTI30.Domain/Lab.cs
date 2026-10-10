namespace RutaTI30.Domain;

/// <summary>Acción disponible en un escenario de laboratorio. Solo produce evidencias simuladas:
/// nunca se ejecutan procesos ni se modifica el sistema.</summary>
public sealed record LabAction
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    /// <summary>"Lectura" = diagnóstico de solo lectura; "Cambio" = acción que en la vida real modificaría el sistema.</summary>
    public required string Kind { get; init; }
    /// <summary>Evidencia simulada que devuelve la acción (clave → valor).</summary>
    public required IReadOnlyList<KeyValuePair<string, string>> Evidence { get; init; }
    /// <summary>Retroalimentación pedagógica mostrada tras elegir la acción.</summary>
    public required string Feedback { get; init; }
    /// <summary>Ponderación: recomendada > 0, neutral = 0, insegura o prematura &lt; 0.</summary>
    public required int ScoreWeight { get; init; }
    /// <summary>Indica si la acción exige permisos elevados o es riesgosa en la vida real.</summary>
    public bool RequiresElevation { get; init; }
}

/// <summary>Escenario de diagnóstico simulado.</summary>
public sealed record LabScenario
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required int Difficulty { get; init; }
    public required string Context { get; init; }
    public required IReadOnlyList<string> Symptoms { get; init; }
    public required IReadOnlyList<KeyValuePair<string, string>> InitialData { get; init; }
    public required IReadOnlyList<LabAction> Actions { get; init; }
    public required string Solution { get; init; }
    public required string EscalationGuidance { get; init; }
    public required IReadOnlyList<string> EvaluationCriteria { get; init; }
    /// <summary>Puntaje máximo alcanzable (suma de pesos positivos).</summary>
    public int MaxScore { get; init; }
}

/// <summary>Intento de laboratorio guardado: decisiones y puntaje resultantes.</summary>
public sealed class LabAttempt
{
    public long Id { get; set; }
    public required string ScenarioId { get; set; }
    public DateTime StartedUtc { get; set; }
    public DateTime? FinishedUtc { get; set; }
    public double Score { get; set; }
    public string DecisionsJson { get; set; } = "[]";
}
