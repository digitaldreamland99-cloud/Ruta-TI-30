namespace RutaTI30.Domain;

/// <summary>Pregunta del banco de entrevistas.</summary>
public sealed record InterviewQuestion
{
    public required string Id { get; init; }
    public required InterviewKind Kind { get; init; }
    public required string Area { get; init; }
    public required string Statement { get; init; }
    /// <summary>Respuesta orientativa; nunca garantiza contratación.</summary>
    public required string ModelAnswer { get; init; }
    public required string Explanation { get; init; }
    public required IReadOnlyList<string> KeyConcepts { get; init; }
    public required IReadOnlyList<string> CommonMistakes { get; init; }
    public required IReadOnlyList<string> EvaluationCriteria { get; init; }
    public IReadOnlyList<string> FollowUps { get; init; } = Array.Empty<string>();
}

/// <summary>Respuesta personal guardada de una pregunta de entrevista.</summary>
public sealed class InterviewAttempt
{
    public long Id { get; set; }
    public required string QuestionId { get; set; }
    public DateTime AttemptedUtc { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    /// <summary>Autoevaluación 1–5 del propio usuario (no es una métrica de contratación).</summary>
    public int? SelfRating { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>Postulación registrada localmente por el usuario (nunca se envía a empleadores).</summary>
public sealed class JobApplication
{
    public long Id { get; set; }
    public required string Company { get; set; }
    public required string Role { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime? DiscoveredUtc { get; set; }
    public DateTime? AppliedUtc { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public JobState State { get; set; } = JobState.PorRevisar;
    public string Contact { get; set; } = string.Empty;
    public DateTime? InterviewUtc { get; set; }
    public DateTime? NextFollowUpUtc { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
}

/// <summary>Seguimiento puntual de una postulación.</summary>
public sealed class FollowUp
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public DateTime AtUtc { get; set; }
    public required string Note { get; set; }
}

/// <summary>Reglas puras de validación de postulaciones (dominio testeable).</summary>
public static class JobRules
{
    public static IReadOnlyList<string> Validate(JobApplication app)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(app.Company))
            errors.Add("La empresa es obligatoria.");
        if (string.IsNullOrWhiteSpace(app.Role))
            errors.Add("El puesto es obligatorio.");
        if (!string.IsNullOrWhiteSpace(app.Url) &&
            !(app.Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
              app.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            errors.Add("El enlace de la vacante debe empezar con http:// o https://.");
        if (app.DiscoveredUtc.HasValue && app.AppliedUtc.HasValue &&
            app.AppliedUtc.Value < app.DiscoveredUtc.Value)
            errors.Add("La fecha de postulación no puede ser anterior a la de descubrimiento.");
        if (app.AppliedUtc.HasValue && app.InterviewUtc.HasValue &&
            app.InterviewUtc.Value < app.AppliedUtc.Value)
            errors.Add("La fecha de entrevista no puede ser anterior a la postulación.");
        return errors;
    }
}
