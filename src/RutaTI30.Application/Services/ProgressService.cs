using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Application.Services;

/// <summary>Estadísticas calculadas exclusivamente a partir de datos persistidos (sin cifras fabricadas).</summary>
public sealed record ProgressStatistics(
    int TotalLessons,
    int Started,
    int Completed,
    int Mastered,
    int EvaluationsTaken,
    double? AverageScore,
    int TotalStudySeconds,
    int StudySecondsToday,
    int TicketsCreated,
    int TicketsResolved,
    IReadOnlyList<string> WeakTopics,
    IReadOnlyList<string> DemonstratedTopics);

public sealed class ProgressService
{
    private readonly ILearningStore _learning;
    private readonly ISessionStore _sessions;
    private readonly ITicketStore _tickets;
    private readonly IClock _clock;

    public ProgressService(ILearningStore learning, ISessionStore sessions, ITicketStore tickets, IClock clock)
    {
        _learning = learning;
        _sessions = sessions;
        _tickets = tickets;
        _clock = clock;
    }

    public ProgressStatistics GetStatistics(IReadOnlyList<Lesson> allLessons)
    {
        ArgumentNullException.ThrowIfNull(allLessons);
        var progress = _learning.GetAllProgress();
        var attempts = _learning.GetAttempts();

        int started = progress.Count(p => p.State != LessonState.NotStarted);
        int completed = progress.Count(p => p.State is LessonState.Completed or LessonState.Mastered);
        int mastered = progress.Count(p => p.State == LessonState.Mastered);

        var scores = attempts.Select(a => a.Score).ToList();
        double? average = scores.Count == 0 ? null : Math.Round(scores.Average(), 2);

        var todayLocal = _clock.LocalNow.Date;
        var sessions = _sessions.GetSessions();
        int todaySeconds = sessions
            .Where(s => s.StartedUtc.ToLocalTime().Date == todayLocal)
            .Sum(s => s.EffectiveSeconds);

        // Temas débiles: de cada lección se toma el último intento; si no aprobó, sus temas fallidos cuentan.
        var weak = new List<string>();
        var demonstrated = new List<string>();
        foreach (var lesson in allLessons)
        {
            var last = attempts.LastOrDefault(a => a.LessonId == lesson.Id);
            if (last is null) continue;
            if (last.Passed)
            {
                demonstrated.AddRange(lesson.Questions.Select(q => q.Topic));
            }
            else
            {
                var failed = DeserializeFailedTopics(last).ToList();
                if (failed.Count == 0)
                    failed = lesson.Questions
                        .Where(q => !IsCorrectInDetails(last.DetailsJson, q))
                        .Select(q => q.Topic).ToList();
                weak.AddRange(failed.Distinct());
            }
        }

        // Lecciones completadas sin dominio también requieren repaso.
        foreach (var p in progress.Where(p => p.State == LessonState.Completed))
        {
            var lesson = allLessons.FirstOrDefault(l => l.Id == p.LessonId);
            if (lesson is not null) weak.AddRange(LessonTopicsOf(lesson));
        }

        bool IsCorrectInDetails(string detailsJson, EvaluationQuestion q) =>
            AttemptDetails.IsCorrect(detailsJson, q.Id);

        return new ProgressStatistics(
            allLessons.Count,
            started,
            completed,
            mastered,
            attempts.Count,
            average,
            _sessions.GetTotalEffectiveSeconds(),
            todaySeconds,
            _tickets.CountAll(),
            _tickets.CountByState(TicketState.Resuelto) + _tickets.CountByState(TicketState.Cerrado),
            weak.Distinct().ToList(),
            demonstrated.Distinct().ToList());
    }

    private static IEnumerable<string> LessonTopicsOf(Lesson lesson) =>
        lesson.Topics.Count > 0 ? lesson.Topics : lesson.Questions.Select(q => q.Topic).Distinct();

    private static IEnumerable<string> DeserializeFailedTopics(EvaluationAttempt attempt) =>
        AttemptDetails.FailedTopics(attempt.DetailsJson);
}
