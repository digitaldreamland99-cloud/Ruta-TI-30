using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class ReviewServiceTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly FakeClock _clock = new();
    private readonly LearningStore _learning;
    private readonly ReviewService _service;
    private readonly Lesson _lesson;
    private readonly string _topic;

    public ReviewServiceTests()
    {
        _learning = new LearningStore(_db.Db);
        _service = new ReviewService(_learning, LessonCatalog.Instance, _clock);
        _lesson = LessonCatalog.Instance.GetByDay(1)!;
        _topic = _lesson.Questions[0].Topic;
    }

    public void Dispose() => _db.Dispose();

    private void Attempt(params string[] failedTopics)
    {
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = _lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = failedTopics.Length == 0 ? 100 : 50,
            Passed = failedTopics.Length == 0,
            DetailsJson = AttemptDetails.Serialize(new Dictionary<string, int?>(), failedTopics)
        });
    }

    [Fact]
    public void SinIntentos_NoHayRepasos()
    {
        Assert.Empty(_service.GetDueReviews());
        Assert.Empty(_service.GetScheduledReviews());
        Assert.Equal(0, _service.PendingCount);
    }

    [Fact]
    public void Fallo_NoVenceDeInmediato_SinoTrasUnDia()
    {
        Attempt(_topic);

        // Recién fallado: aún no vence, pero queda programado dentro de 14 días.
        Assert.Empty(_service.GetDueReviews());
        var scheduled = _service.GetScheduledReviews();
        var item = Assert.Single(scheduled);
        Assert.Equal(_topic, item.Topic);
        Assert.Equal(1, item.IntervalDays);
        Assert.Equal(_clock.UtcNow.AddDays(1), item.NextDueUtc);

        _clock.Advance(TimeSpan.FromDays(1));
        var due = Assert.Single(_service.GetDueReviews());
        Assert.Equal(_topic, due.Topic);
        Assert.Equal(1, _service.PendingCount);
    }

    [Fact]
    public void FalloRepetido_ProgramaConIntervaloCreciente()
    {
        Attempt(_topic);          // fallo 1 -> repaso a 1 día
        _clock.Advance(TimeSpan.FromDays(1));
        Attempt(_topic);          // fallo 2 -> repaso a 2 días

        var item = Assert.Single(_service.GetScheduledReviews());
        Assert.Equal(2, item.Failures);
        Assert.Equal(2, item.IntervalDays);
        Assert.Equal(_clock.UtcNow.AddDays(2), item.NextDueUtc);

        _clock.Advance(TimeSpan.FromDays(2));
        Assert.Single(_service.GetDueReviews());
    }

    [Fact]
    public void AciertoTrasFallo_AlargaElIntervalo()
    {
        Attempt(_topic);                      // fallo -> 1 día
        _clock.Advance(TimeSpan.FromDays(1));
        Attempt();                            // acierto -> 3 días

        var item = Assert.Single(_service.GetScheduledReviews());
        Assert.Equal(0, item.Failures);
        Assert.Equal(1, item.Successes);
        Assert.Equal(3, item.IntervalDays);

        _clock.Advance(TimeSpan.FromDays(2));
        Assert.Empty(_service.GetDueReviews()); // aún no vence

        _clock.Advance(TimeSpan.FromDays(1));
        Assert.Single(_service.GetDueReviews()); // vence a los 3 días
    }

    [Fact]
    public void TemasNoFallados_NoEntranEnLaCola()
    {
        // Solo falla un tema: los demás no deben aparecer como repaso pendiente.
        var otroTema = _lesson.Questions[^1].Topic;
        if (string.Equals(otroTema, _topic, StringComparison.OrdinalIgnoreCase))
            return; // lección con un único tema: no hay distinción que comprobar

        Attempt(_topic);

        var item = Assert.Single(_service.GetScheduledReviews());
        Assert.Equal(_topic, item.Topic);
        Assert.DoesNotContain(_service.GetScheduledReviews(), i => i.Topic == otroTema);
    }

    [Fact]
    public void TemaNoFalladoEnNingunIntento_NoAparece()
    {
        Attempt(); // todo correcto: nada que repasar
        Assert.Empty(_service.GetDueReviews());
        Assert.Empty(_service.GetScheduledReviews());
    }

    [Fact]
    public void LeccionDesconocida_ODetallesIlegibles_NoRompenElCalculo()
    {
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = "leccion-inexistente",
            AttemptedUtc = _clock.UtcNow,
            Score = 40,
            Passed = false,
            DetailsJson = "esto-no-es-json"
        });
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = _lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 40,
            Passed = false,
            DetailsJson = "{corrupto"
        });

        Assert.Empty(_service.GetDueReviews());
        Assert.Equal(0, _service.PendingCount);
    }

    [Fact]
    public void ElRepasoSeDerivaDeIntentosPersistidos_NoDeEstadoEnMemoria()
    {
        Attempt(_topic);

        // Nueva instancia sobre la misma base (como al reiniciar la aplicación).
        var otra = new ReviewService(_learning, LessonCatalog.Instance, _clock);
        Assert.Empty(otra.GetDueReviews());
        Assert.Single(otra.GetScheduledReviews());

        _clock.Advance(TimeSpan.FromDays(1));
        var due = Assert.Single(otra.GetDueReviews());
        Assert.Equal(_topic, due.Topic);
    }
}
