using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class ProgressServiceTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly FakeClock _clock = new();
    private readonly LearningStore _learning;
    private readonly SessionStore _sessions;
    private readonly TicketStore _tickets;
    private readonly ProgressService _service;

    public ProgressServiceTests()
    {
        _learning = new LearningStore(_db.Db);
        _sessions = new SessionStore(_db.Db);
        _tickets = new TicketStore(_db.Db);
        _service = new ProgressService(_learning, _sessions, _tickets, _clock);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public void Estadisticas_SinDatos_NoInventaCifras()
    {
        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Equal(0, stats.Started);
        Assert.Equal(0, stats.Completed);
        Assert.Equal(0, stats.Mastered);
        Assert.Equal(0, stats.EvaluationsTaken);
        Assert.Null(stats.AverageScore);
        Assert.Equal(0, stats.TotalStudySeconds);
        Assert.Empty(stats.WeakTopics);
        Assert.Empty(stats.DemonstratedTopics);
    }

    [Fact]
    public void Estadisticas_CalculaDesdeDatosPersistidos()
    {
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-01", State = LessonState.InProgress, StartedUtc = _clock.UtcNow });
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-02", State = LessonState.Completed, StartedUtc = _clock.UtcNow, CompletedUtc = _clock.UtcNow });
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-03", State = LessonState.Mastered, StartedUtc = _clock.UtcNow, CompletedUtc = _clock.UtcNow, MasteredUtc = _clock.UtcNow });

        _learning.AddAttempt(new EvaluationAttempt { LessonId = "leccion-01", AttemptedUtc = _clock.UtcNow, Score = 60, Passed = false, DetailsJson = "{\"failedTopics\":[\"TemaA\"]}" });
        _learning.AddAttempt(new EvaluationAttempt { LessonId = "leccion-02", AttemptedUtc = _clock.UtcNow, Score = 100, Passed = true, DetailsJson = "{\"failedTopics\":[]}" });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Equal(3, stats.Started);
        Assert.Equal(2, stats.Completed);
        Assert.Equal(1, stats.Mastered);
        Assert.Equal(2, stats.EvaluationsTaken);
        Assert.Equal(80, stats.AverageScore); // (60 + 100) / 2
        Assert.NotEmpty(stats.WeakTopics);
        Assert.NotEmpty(stats.DemonstratedTopics);
    }

    [Fact]
    public void Estadisticas_TiempoDeEstudioSeSumaDeSesiones()
    {
        _sessions.SaveSession(new StudySession { StartedUtc = _clock.UtcNow, EndedUtc = _clock.UtcNow.AddMinutes(45), EffectiveSeconds = 2700, BlockKind = ScheduleKind.Aprendizaje });
        _sessions.SaveSession(new StudySession { StartedUtc = _clock.UtcNow.AddDays(-2), EndedUtc = _clock.UtcNow.AddDays(-2).AddMinutes(20), EffectiveSeconds = 1200, BlockKind = ScheduleKind.Laboratorio });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Equal(3900, stats.TotalStudySeconds);
        Assert.Equal(2700, stats.StudySecondsToday);
    }

    [Fact]
    public void Estadisticas_LeccionCompletadaSinDominio_QuedaEnRepaso()
    {
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-01", State = LessonState.Completed, StartedUtc = _clock.UtcNow, CompletedUtc = _clock.UtcNow });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.NotEmpty(stats.WeakTopics);
        Assert.Empty(stats.DemonstratedTopics);
    }

    [Fact]
    public void Estadisticas_CuentanTicketsReales()
    {
        _tickets.AddWithHistory(new Ticket
        {
            Id = "INC-1",
            CreatedUtc = _clock.UtcNow,
            UpdatedUtc = _clock.UtcNow,
            Reporter = "Ana",
            Description = "Equipo sin encendido desde la mañana",
            State = TicketState.Nuevo
        }, new TicketHistoryEntry { TicketId = "INC-1", AtUtc = _clock.UtcNow, Action = "Alta" });

        _tickets.AddWithHistory(new Ticket
        {
            Id = "INC-2",
            CreatedUtc = _clock.UtcNow,
            UpdatedUtc = _clock.UtcNow,
            Reporter = "Luis",
            Description = "Impresora sin respuesta en piso 2",
            State = TicketState.Resuelto
        }, new TicketHistoryEntry { TicketId = "INC-2", AtUtc = _clock.UtcNow, Action = "Alta" });

        var stats = _service.GetStatistics(LessonCatalog.Instance.Lessons);

        Assert.Equal(2, stats.TicketsCreated);
        Assert.Equal(1, stats.TicketsResolved);
    }
}
