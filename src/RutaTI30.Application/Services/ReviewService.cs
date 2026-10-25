using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;

namespace RutaTI30.Application.Services;

/// <summary>Concepto en el calendario de repaso: se deriva únicamente de intentos realmente
/// registrados (nunca de estimaciones). <see cref="NextDueUtc"/> indica cuándo debe volver
/// a presentarse el concepto.</summary>
public sealed record ReviewItem(
    string LessonId,
    int Day,
    string LessonTitle,
    string Topic,
    int Failures,
    int Successes,
    int IntervalDays,
    DateTime NextDueUtc,
    string Reason);

/// <summary>Repaso espaciado sencillo y explicable: los conceptos fallados vuelven a
/// presentarse antes que los dominados, con intervalos crecientes.
///
/// Reglas (documentadas, no mágicas):
///  - Tras un fallo en un tema: el próximo repaso es a los 1, 2, 4, 8… días (tope 14).
///  - Tras aciertos consecutivos del mismo tema: el intervalo se alarga a 3, 7, 14, 30 días.
///  - Un tema nunca fallado no entra en la cola de repaso.
///  - Todo procede de <c>evaluation_attempts.details_json</c> (temas fallados por intento).</summary>
public sealed class ReviewService
{
    private static readonly int[] FailIntervals = { 1, 2, 4, 8, 14, 14 };
    private static readonly int[] PassIntervals = { 3, 7, 14, 30, 30 };

    private readonly ILearningStore _learning;
    private readonly ILessonCatalog _catalog;
    private readonly IClock _clock;

    public ReviewService(ILearningStore learning, ILessonCatalog catalog, IClock clock)
    {
        _learning = learning;
        _catalog = catalog;
        _clock = clock;
    }

    /// <summary>Conceptos con repaso vencido ahora mismo, ordenados por urgencia.</summary>
    public IReadOnlyList<ReviewItem> GetDueReviews()
    {
        var now = _clock.UtcNow;
        return Build()
            .Where(i => i.NextDueUtc <= now)
            .OrderBy(i => i.NextDueUtc)
            .ThenBy(i => i.Topic, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>Conceptos programados en los próximos días (para mostrar el plan de repaso).</summary>
    public IReadOnlyList<ReviewItem> GetScheduledReviews(int horizonDays = 14)
    {
        var now = _clock.UtcNow;
        var limit = now.AddDays(horizonDays);
        return Build()
            .Where(i => i.NextDueUtc > now && i.NextDueUtc <= limit)
            .OrderBy(i => i.NextDueUtc)
            .ThenBy(i => i.Topic, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>Número de conceptos con repaso vencido (para el panel).</summary>
    public int PendingCount => GetDueReviews().Count;

    private sealed class TopicState
    {
        public int Failures;
        public int Successes;
        public DateTime NextDueUtc;
        public int IntervalDays;
        public bool EverFailed;
        public int Day;
        public string LessonId = string.Empty;
        public string LessonTitle = string.Empty;
        public string Topic = string.Empty;
        public DateTime LastUpdateUtc;
    }

    private IReadOnlyList<ReviewItem> Build()
    {
        var attempts = _learning.GetAttempts(); // ordenados por id (cronológicos)
        var state = new Dictionary<string, TopicState>(StringComparer.Ordinal);

        foreach (var attempt in attempts)
        {
            var lesson = _catalog.GetById(attempt.LessonId);
            if (lesson is null || lesson.Questions.Count == 0) continue;

            var failed = new HashSet<string>(
                AttemptDetails.FailedTopics(attempt.DetailsJson),
                StringComparer.OrdinalIgnoreCase);

            foreach (var topic in lesson.Questions.Select(q => q.Topic).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var key = lesson.Id + "|" + topic;
                var wasFailed = failed.Contains(topic);

                if (!state.TryGetValue(key, out var st))
                {
                    if (!wasFailed) continue; // sin historial de fallo: no hay nada que repasar
                    st = new TopicState
                    {
                        LessonId = lesson.Id,
                        Day = lesson.Day,
                        LessonTitle = lesson.Title,
                        Topic = topic
                    };
                    state[key] = st;
                }

                if (wasFailed)
                {
                    st.Failures++;
                    st.Successes = 0;
                    st.EverFailed = true;
                    st.IntervalDays = FailIntervals[Math.Min(st.Failures - 1, FailIntervals.Length - 1)];
                }
                else
                {
                    st.Successes++;
                    st.Failures = 0;
                    st.IntervalDays = PassIntervals[Math.Min(st.Successes - 1, PassIntervals.Length - 1)];
                }

                st.LastUpdateUtc = attempt.AttemptedUtc;
                st.NextDueUtc = attempt.AttemptedUtc.AddDays(st.IntervalDays);
            }
        }

        return state.Values
            .Where(s => s.EverFailed)
            .Select(s => new ReviewItem(
                s.LessonId,
                s.Day,
                s.LessonTitle,
                s.Topic,
                s.Failures,
                s.Successes,
                s.IntervalDays,
                s.NextDueUtc,
                BuildReason(s)))
            .ToList();
    }

    private static string BuildReason(TopicState s)
    {
        if (s.Failures > 0)
            return s.Failures == 1
                ? $"Falló en el último intento; repaso a los {s.IntervalDays} día(s)."
                : $"Falló {s.Failures} veces seguidas; repaso a los {s.IntervalDays} día(s).";
        return s.Successes >= 3
            ? $"Aciertos consecutivos ({s.Successes}); refuerzo de mantenimiento cada {s.IntervalDays} días."
            : $"Repasado correctamente ({s.Successes} vez/veces); siguiente repaso en {s.IntervalDays} días.";
    }
}
