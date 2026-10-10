namespace RutaTI30.Domain;

/// <summary>Un día del programa de 30 días.</summary>
public sealed record LearningDay
{
    public required int Day { get; init; }
    public required LearningBlock Block { get; init; }
    public required string Title { get; init; }
    public required string Objective { get; init; }
    public required int EstimatedMinutes { get; init; }
    public required IReadOnlyList<string> Topics { get; init; }
}

/// <summary>Pregunta de evaluación de opción múltiple con explicación razonada.</summary>
public sealed record EvaluationQuestion
{
    public required string Id { get; init; }
    public required string Statement { get; init; }
    public required IReadOnlyList<string> Options { get; init; }
    /// <summary>Índice de la opción correcta.</summary>
    public required int CorrectIndex { get; init; }
    public required string Explanation { get; init; }
    /// <summary>Competencia o tema asociado (para detectar temas débiles).</summary>
    public required string Topic { get; init; }
}

/// <summary>Lección completa con la estructura pedagógica obligatoria.</summary>
public sealed record Lesson
{
    public required string Id { get; init; }
    public required int Day { get; init; }
    public required LearningBlock Block { get; init; }
    public required string Title { get; init; }
    /// <summary>1. Objetivo medible.</summary>
    public required string Objective { get; init; }
    /// <summary>2. Nivel de dificultad (1–5).</summary>
    public required int Difficulty { get; init; }
    /// <summary>3. Duración estimada en minutos.</summary>
    public required int EstimatedMinutes { get; init; }
    /// <summary>4. Requisitos previos.</summary>
    public required IReadOnlyList<string> Prerequisites { get; init; }
    /// <summary>5. Explicación para principiantes.</summary>
    public required string BeginnerExplanation { get; init; }
    /// <summary>6. Conceptos y vocabulario (término → definición).</summary>
    public required IReadOnlyList<KeyValuePair<string, string>> Concepts { get; init; }
    /// <summary>7. Ejemplo de un entorno laboral.</summary>
    public required string WorkplaceExample { get; init; }
    /// <summary>8. Procedimiento paso a paso.</summary>
    public required IReadOnlyList<string> Procedure { get; init; }
    /// <summary>9. Errores frecuentes.</summary>
    public required IReadOnlyList<string> CommonMistakes { get; init; }
    /// <summary>10. Ejercicio guiado.</summary>
    public required string GuidedExercise { get; init; }
    /// <summary>11. Ejercicio independiente.</summary>
    public required string IndependentExercise { get; init; }
    /// <summary>12. Preguntas de evaluación.</summary>
    public required IReadOnlyList<EvaluationQuestion> Questions { get; init; }
    /// <summary>15. Resumen.</summary>
    public required string Summary { get; init; }
    /// <summary>16. Recomendaciones de repaso.</summary>
    public required string ReviewRecommendations { get; init; }
    /// <summary>17. Referencias útiles (complementarias; el contenido esencial vive en la app).</summary>
    public required IReadOnlyList<string> References { get; init; }
    /// <summary>14. Criterios de aprobación.</summary>
    public required string ApprovalCriteria { get; init; }
    /// <summary>Temas/competencias representativos (para repaso y estadísticas).</summary>
    public IReadOnlyList<string> Topics { get; init; } = Array.Empty<string>();
}

/// <summary>Progreso persistido de una lección por el usuario.</summary>
public sealed class LessonProgress
{
    public required string LessonId { get; set; }
    public LessonState State { get; set; } = LessonState.NotStarted;
    public DateTime? StartedUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }
    public DateTime? MasteredUtc { get; set; }
    /// <summary>Último puntaje obtenido en la evaluación (0–100), nulo si nunca se evaluó.</summary>
    public double? LastScore { get; set; }
}

/// <summary>Intento de evaluación de una lección (historial conservado para ver evolución).</summary>
public sealed class EvaluationAttempt
{
    public long Id { get; set; }
    public required string LessonId { get; set; }
    public DateTime AttemptedUtc { get; set; }
    public double Score { get; set; }
    public bool Passed { get; set; }
    /// <summary>Detalle JSON: respuestas y temas fallados.</summary>
    public string DetailsJson { get; set; } = "{}";
}
