using RutaTI30.Domain;

namespace RutaTI30.Application.Services;

/// <summary>Resultado calculado de una evaluación.</summary>
public sealed record EvaluationResult(
    int Correct,
    int Total,
    double Score,
    bool Passed,
    IReadOnlyList<string> FailedTopics,
    int UnansweredCount);

/// <summary>Evalúa respuestas reales contra las preguntas de la lección. No mide actividad: mide resultados.</summary>
public sealed class EvaluationService
{
    public const double DefaultThreshold = 80.0;

    /// <summary>Umbral configurable por el usuario (preferencia "evaluation.threshold").</summary>
    private readonly double _threshold;

    public EvaluationService(double? threshold = null)
    {
        _threshold = threshold is > 0 and <= 100 ? threshold.Value : DefaultThreshold;
    }

    public double Threshold => _threshold;

    /// <param name="selectedIndexes">Índice de opción elegida por pregunta; null = sin responder.</param>
    public EvaluationResult Evaluate(Lesson lesson, IReadOnlyList<int?> selectedIndexes)
    {
        ArgumentNullException.ThrowIfNull(lesson);
        ArgumentNullException.ThrowIfNull(selectedIndexes);
        if (selectedIndexes.Count != lesson.Questions.Count)
            throw new ArgumentException(
                $"Se esperaban {lesson.Questions.Count} respuestas y se recibieron {selectedIndexes.Count}.");

        int correct = 0, unanswered = 0;
        var failedTopics = new List<string>();

        for (int i = 0; i < lesson.Questions.Count; i++)
        {
            var q = lesson.Questions[i];
            var selected = selectedIndexes[i];
            if (selected is null)
            {
                unanswered++;
                failedTopics.Add(q.Topic);
            }
            else if (selected.Value == q.CorrectIndex)
            {
                correct++;
            }
            else
            {
                failedTopics.Add(q.Topic);
            }
        }

        int total = lesson.Questions.Count;
        double score = total == 0 ? 0 : Math.Round(correct * 100d / total, 2);
        return new EvaluationResult(
            correct,
            total,
            score,
            total > 0 && score >= _threshold,
            failedTopics.Distinct().ToList(),
            unanswered);
    }
}
