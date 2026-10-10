using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Application.Services;

/// <summary>Panel de inicio calculado desde datos persistidos (nada de cifras predeterminadas).</summary>
public sealed record DashboardData(
    DateTime LocalDate,
    int CurrentDay,
    string DayTitle,
    string DayObjective,
    Lesson? CurrentLesson,
    LessonState CurrentLessonState,
    bool HasAnyData,
    int StartedCount,
    int CompletedCount,
    int MasteredCount,
    int TotalLessons,
    int StudySecondsToday,
    int StudySecondsTotal,
    IReadOnlyList<string> PendingActivities,
    EvaluationAttempt? LastAttempt,
    IReadOnlyList<string> ReviewTopics,
    int SessionsLast7Days,
    int ActiveDaysLast7Days);

public sealed class DashboardService
{
    public const string CurrentDayKey = "program.current_day";
    public const int MaxDay = 30;

    private readonly ILearningStore _learning;
    private readonly ISessionStore _sessions;
    private readonly IPreferenceStore _preferences;
    private readonly IClock _clock;

    public DashboardService(ILearningStore learning, ISessionStore sessions, IPreferenceStore preferences, IClock clock)
    {
        _learning = learning;
        _sessions = sessions;
        _preferences = preferences;
        _clock = clock;
    }

    /// <summary>Día académico guardado; NO cambia con la fecha calendario, solo por acción explícita del usuario.</summary>
    public int GetCurrentDay() => Math.Clamp(_preferences.GetInt(CurrentDayKey, 1), 1, MaxDay);

    public void SetCurrentDay(int day) => _preferences.SetInt(CurrentDayKey, Math.Clamp(day, 1, MaxDay));

    public DashboardData GetDashboard(IReadOnlyList<Lesson> lessons)
    {
        ArgumentNullException.ThrowIfNull(lessons);
        int day = GetCurrentDay();
        var lesson = lessons.FirstOrDefault(l => l.Day == day);
        var progress = _learning.GetAllProgress();
        var attempts = _learning.GetAttempts();
        var sessions = _sessions.GetSessions();

        var state = lesson is null
            ? LessonState.NotStarted
            : progress.FirstOrDefault(p => p.LessonId == lesson.Id)?.State ?? LessonState.NotStarted;

        var today = _clock.LocalNow.Date;
        int todaySeconds = sessions
            .Where(s => s.StartedUtc.ToLocalTime().Date == today)
            .Sum(s => s.EffectiveSeconds);

        var from7 = _clock.LocalNow.Date.AddDays(-6);
        var recent = sessions
            .Where(s => s.StartedUtc.ToLocalTime().Date >= from7)
            .ToList();

        var pending = BuildPendingActivities(lesson, state);

        var reviewTopics = BuildReviewTopics(lessons, progress, attempts);

        bool hasData = progress.Count > 0 || sessions.Count > 0 || attempts.Count > 0;

        return new DashboardData(
            _clock.LocalNow,
            day,
            lesson?.Title ?? $"Día {day}",
            lesson?.Objective ?? "Aún no hay contenido cargado para este día.",
            lesson,
            state,
            hasData,
            progress.Count(p => p.State != LessonState.NotStarted),
            progress.Count(p => p.State is LessonState.Completed or LessonState.Mastered),
            progress.Count(p => p.State == LessonState.Mastered),
            lessons.Count,
            todaySeconds,
            sessions.Sum(s => s.EffectiveSeconds),
            pending,
            attempts.LastOrDefault(),
            reviewTopics,
            recent.Count,
            recent.Select(s => s.StartedUtc.ToLocalTime().Date).Distinct().Count());
    }

    private static IReadOnlyList<string> BuildPendingActivities(Lesson? lesson, LessonState state)
    {
        if (lesson is null) return new[] { "No hay lección asignada a este día." };
        return state switch
        {
            LessonState.NotStarted => new[]
            {
                $"Iniciar la lección del día {lesson.Day}: {lesson.Title}",
                "Leer la explicación y los conceptos",
                "Realizar el ejercicio guiado"
            },
            LessonState.InProgress => new[]
            {
                "Completar el ejercicio guiado y el independiente",
                "Presentar la evaluación de la lección",
                "Revisar las soluciones explicadas"
            },
            LessonState.Completed => new[]
            {
                "Repasar los conceptos para alcanzar el dominio",
                "Repetir la evaluación (se conserva el historial de intentos)"
            },
            _ => new[]
            {
                "Lección dominada: continuar con el siguiente día o repasar temas débiles"
            }
        };
    }

    private static IReadOnlyList<string> BuildReviewTopics(
        IReadOnlyList<Lesson> lessons,
        IReadOnlyList<LessonProgress> progress,
        IReadOnlyList<EvaluationAttempt> attempts)
    {
        var topics = new List<string>();
        foreach (var lesson in lessons)
        {
            var last = attempts.LastOrDefault(a => a.LessonId == lesson.Id);
            var state = progress.FirstOrDefault(p => p.LessonId == lesson.Id)?.State ?? LessonState.NotStarted;
            if (last is { Passed: false })
            {
                var failed = AttemptDetails.FailedTopics(last.DetailsJson);
                topics.AddRange(failed.Count > 0
                    ? failed
                    : lesson.Questions.Where(q => !AttemptDetails.IsCorrect(last.DetailsJson, q.Id))
                        .Select(q => q.Topic));
            }
            else if (last is null && state == LessonState.Completed)
            {
                topics.AddRange(lesson.Topics.Count > 0 ? lesson.Topics : lesson.Questions.Select(q => q.Topic));
            }
        }
        return topics.Distinct().Take(8).ToList();
    }
}
