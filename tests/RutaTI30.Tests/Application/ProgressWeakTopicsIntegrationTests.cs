using System.Collections.Generic;
using System.Text.Json;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class ProgressWeakTopicsIntegrationTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly FakeClock _clock = new();
    private readonly LearningStore _learning;
    private readonly SessionStore _sessions;
    private readonly TicketStore _tickets;
    private readonly ProgressService _service;

    public ProgressWeakTopicsIntegrationTests()
    {
        _learning = new LearningStore(_db.Db);
        _sessions = new SessionStore(_db.Db);
        _tickets = new TicketStore(_db.Db);
        _service = new ProgressService(_learning, _sessions, _tickets, _clock);
    }

    public void Dispose() => _db.Dispose();

    /// <summary>Intento con temas fallidos explícitos (reduce al mínimo la dependencia del modo de serialización).</summary>
    private static string DetailsWithFailedTopics(string topic)
    {
        return JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["answers"] = new Dictionary<string, int?>(),
            ["correct"] = new Dictionary<string, bool>(),
            ["failedTopics"] = new[] { topic }
        });
    }

    /// <summary>Intento evaluado con corrección explícita por pregunta: verdadero = acierto, falso = error.</summary>
    private static string DetailsWithEvaluation(
        string questionId,
        int? chosenIndex,
        bool correct)
    {
        var answers = new Dictionary<string, int?> { [questionId] = chosenIndex };
        var results = new Dictionary<string, bool> { [questionId] = correct };
        return JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["answers"] = answers,
            ["correct"] = results,
            ["failedTopics"] = new string[0]
        });
    }

    [Fact]
    public void LeccionSinIntento_NoGeneraTemasDébilesNiDemostrados()
    {
        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Empty(stats.WeakTopics);
        Assert.Empty(stats.DemonstratedTopics);
    }

    [Fact]
    public void LeccionAprobadaConFailedTopics_DemuestraTemas()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topic = lesson.Questions[0].Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 100,
            Passed = true,
            DetailsJson = DetailsWithFailedTopics(topic)
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Contains(topic, stats.DemonstratedTopics);
        Assert.DoesNotContain(topic, stats.WeakTopics);
    }

    [Fact]
    public void LeccionNoAprobadaConFailedTopics_AñadeTemasDebiles()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topic = lesson.Questions[0].Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 50,
            Passed = false,
            DetailsJson = DetailsWithFailedTopics(topic)
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Contains(topic, stats.WeakTopics);
        Assert.DoesNotContain(topic, stats.DemonstratedTopics);
    }

    [Fact]
    public void RespuestaIncorrectaConCorreccionExplicitaSíGeneraTemaDebil()
    {
        // Caso que antes era defectuoso: una respuesta incorrecta ya no se confunde con acierto.
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var q = lesson.Questions[0];
        var topic = q.Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 50,
            Passed = false,
            DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex + 1, correct: false)
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Contains(topic, stats.WeakTopics);
        Assert.DoesNotContain(topic, stats.DemonstratedTopics);
    }

    [Fact]
    public void RespuestaCorrectaConCorreccionExplicitaNoTieneTemaDebil()
    {
        // Una respuesta correcta registrada como tal no genera tema débil.
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var q = lesson.Questions[0];
        var topic = q.Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 100,
            Passed = true,
            DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex, correct: true)
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.DoesNotContain(topic, stats.WeakTopics);
        Assert.Contains(topic, stats.DemonstratedTopics);
    }

    [Fact]
    public void SinCorreccionExplicitada_NoSeInventaAciertoNIError()
    {
        // Un intento sin registro de corrección no se trata como acierto ni como error:
        // ProgressService no lo utiliza para decidir temas débiles ni demostrados.
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topic = lesson.Questions[0].Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 50,
            Passed = false,
            DetailsJson = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["answers"] = new Dictionary<string, int?> { [lesson.Questions[0].Id] = 1 },
                ["failedTopics"] = new string[0]
            })
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.DoesNotContain(topic, stats.WeakTopics);
        Assert.DoesNotContain(topic, stats.DemonstratedTopics);
    }

    [Fact]
    public void DetallesVacioONulo_NoGeneraFalsosTemasDebilesNiDemostrados()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var lessonTopics = lesson.Questions.Select(q => q.Topic).ToHashSet();

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 0,
            Passed = false,
            DetailsJson = "{}"
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.DoesNotContain(stats.DemonstratedTopics, t => lessonTopics.Contains(t));
        Assert.DoesNotContain(stats.WeakTopics, t => lessonTopics.Contains(t));
    }

    [Fact]
    public void DetallesMalformadoNoRompeCalculoTemasDebiles()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topic = lesson.Questions[0].Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 50,
            Passed = false,
            DetailsJson = "{sin json válido"
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.DoesNotContain(topic, stats.WeakTopics);
        Assert.DoesNotContain(topic, stats.DemonstratedTopics);
    }

    [Fact]
    public void ActualizacionUltimoIntento_TemasDebilesReflejanUltimoIntento()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topic = lesson.Questions[0].Topic;

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 50,
            Passed = false,
            DetailsJson = DetailsWithFailedTopics(topic)
        });

        Assert.Contains(topic, _service.GetStatistics(LessonCatalog.Instance.Lessons).WeakTopics);

        _clock.Advance(TimeSpan.FromMinutes(5));
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 100,
            Passed = true,
            DetailsJson = DetailsWithEvaluation(lesson.Questions[0].Id, lesson.Questions[0].CorrectIndex, correct: true)
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.DoesNotContain(topic, stats.WeakTopics);
        Assert.Contains(topic, stats.DemonstratedTopics);
    }

    [Fact]
    public void CompletadaSinDominio_EstimulaTemasDebiles()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;

        _learning.SaveProgress(new LessonProgress
        {
            LessonId = lesson.Id,
            State = LessonState.Completed,
            StartedUtc = _clock.UtcNow,
            CompletedUtc = _clock.UtcNow
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);
        var lessonTopics = lesson.Topics.Count > 0
            ? lesson.Topics
            : lesson.Questions.Select(q => q.Topic).Distinct();

        // Lección completada sin dominio sigue requiriendo repaso (cada tema de lección entra en débiles).
        foreach (var topic in lessonTopics)
            Assert.Contains(topic, stats.WeakTopics);
    }

    [Fact]
    public void PromedioSimple_YTemasConDosIntentosOpuestos()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topic = lesson.Questions[0].Topic;
        var q = lesson.Questions[0];

        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 60,
            Passed = false,
            DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex + 1, correct: false)
        });

        _clock.Advance(TimeSpan.FromMinutes(10));
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 100,
            Passed = true,
            DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex, correct: true)
        });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Equal(2, stats.EvaluationsTaken);
        Assert.Equal(80, stats.AverageScore); // (60 + 100) / 2
        Assert.Contains(topic, stats.DemonstratedTopics);
        Assert.DoesNotContain(topic, stats.WeakTopics);
    }

    [Fact]
    public void MostradoEnPanelTemasDebiles_CantidadEsperada()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var topics = lesson.Questions.Select(q => q.Topic).ToList();
        var q = lesson.Questions[0];

        foreach (var topic in topics)
        {
            _learning.AddAttempt(new EvaluationAttempt
            {
                LessonId = lesson.Id,
                AttemptedUtc = _clock.UtcNow,
                Score = 50,
                Passed = false,
                DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex + 1, correct: false)
            });
        }

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Equal(topics.Count, stats.WeakTopics.Count);
        foreach (var topic in topics)
            Assert.Contains(topic, stats.WeakTopics);
    }

    [Fact]
    public void ReviewService_DistingueCorrectoDeIncorrectoDesdeIntentosEvaluados()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var q = lesson.Questions[0];
        var topic = q.Topic;

        // Fallo explícito -> entra en repaso programado.
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 50,
            Passed = false,
            DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex + 1, correct: false)
        });

        var service = new ReviewService(_learning, LessonCatalog.Instance, _clock);

        Assert.Empty(service.GetDueReviews());
        Assert.Contains(service.GetScheduledReviews(), item => item.Topic == topic);
    }

    [Fact]
    public void ReviewService_NoProgramaTemaSinFalloRegistrado()
    {
        var lesson = LessonCatalog.Instance.GetByDay(1)!;
        var q = lesson.Questions[0];

        // Acierto explícito: el tema no entra en cola de repaso.
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = lesson.Id,
            AttemptedUtc = _clock.UtcNow,
            Score = 100,
            Passed = true,
            DetailsJson = DetailsWithEvaluation(q.Id, q.CorrectIndex, correct: true)
        });

        var service = new ReviewService(_learning, LessonCatalog.Instance, _clock);

        Assert.Empty(service.GetDueReviews());
        Assert.Empty(service.GetScheduledReviews());
    }
}
