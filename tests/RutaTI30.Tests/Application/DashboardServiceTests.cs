using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class DashboardServiceTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly FakeClock _clock = new();
    private readonly LearningStore _learning;
    private readonly SessionStore _sessions;
    private readonly PreferenceStore _preferences;
    private readonly DashboardService _dashboard;

    public DashboardServiceTests()
    {
        _learning = new LearningStore(_db.Db);
        _sessions = new SessionStore(_db.Db);
        _preferences = new PreferenceStore(_db.Db);
        _dashboard = new DashboardService(_learning, _sessions, _preferences, _clock);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public void Panel_SinDatos_MuestraBienvenida()
    {
        var data = _dashboard.GetDashboard(LessonCatalog.Instance.Lessons);

        Assert.False(data.HasAnyData);
        Assert.Equal(1, data.CurrentDay);
        Assert.NotEmpty(data.PendingActivities);
    }

    [Fact]
    public void DiaAcademico_NoCambiaConLaFecha()
    {
        _dashboard.SetCurrentDay(5);
        Assert.Equal(5, _dashboard.GetCurrentDay());

        // Pasan 10 días de calendario sin estudiar.
        _clock.Advance(TimeSpan.FromDays(10));

        Assert.Equal(5, _dashboard.GetCurrentDay());
    }

    [Fact]
    public void DiaAcademico_FueraDeRango_Seaclampa()
    {
        _dashboard.SetCurrentDay(999);
        Assert.Equal(30, _dashboard.GetCurrentDay());

        _dashboard.SetCurrentDay(0);
        Assert.Equal(1, _dashboard.GetCurrentDay());
    }

    [Fact]
    public void Panel_CuentaProgresoReal()
    {
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-01", State = LessonState.InProgress, StartedUtc = _clock.UtcNow });
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-02", State = LessonState.Completed, StartedUtc = _clock.UtcNow, CompletedUtc = _clock.UtcNow });
        _learning.SaveProgress(new LessonProgress { LessonId = "leccion-03", State = LessonState.Mastered, StartedUtc = _clock.UtcNow, CompletedUtc = _clock.UtcNow, MasteredUtc = _clock.UtcNow });

        var data = _dashboard.GetDashboard(LessonCatalog.Instance.Lessons);

        Assert.True(data.HasAnyData);
        Assert.Equal(3, data.StartedCount);
        Assert.Equal(2, data.CompletedCount);
        Assert.Equal(1, data.MasteredCount);
        Assert.Equal(30, data.TotalLessons);
    }

    [Fact]
    public void Panel_TiempoDeHoy_VieneDeSesiones()
    {
        _sessions.SaveSession(new StudySession
        {
            StartedUtc = _clock.UtcNow,
            EndedUtc = _clock.UtcNow.AddMinutes(30),
            EffectiveSeconds = 1800,
            BlockKind = ScheduleKind.Aprendizaje
        });

        var data = _dashboard.GetDashboard(LessonCatalog.Instance.Lessons);

        Assert.Equal(1800, data.StudySecondsToday);
        Assert.Equal(1800, data.StudySecondsTotal);
        Assert.Equal(1, data.SessionsLast7Days);
        Assert.Equal(1, data.ActiveDaysLast7Days);
    }

    [Fact]
    public void Panel_TiempoDeOtroDia_NoCuentaComoHoy()
    {
        _sessions.SaveSession(new StudySession
        {
            StartedUtc = _clock.UtcNow.AddDays(-3),
            EndedUtc = _clock.UtcNow.AddDays(-3).AddMinutes(30),
            EffectiveSeconds = 1800,
            BlockKind = ScheduleKind.Repaso
        });

        var data = _dashboard.GetDashboard(LessonCatalog.Instance.Lessons);

        Assert.Equal(0, data.StudySecondsToday);
        Assert.Equal(1800, data.StudySecondsTotal);
        Assert.Equal(1, data.SessionsLast7Days); // los últimos 7 días incluyen hace 3 días
    }

    [Fact]
    public void Panel_UltimoIntento_ReflejaElUltimoReal()
    {
        _learning.AddAttempt(new EvaluationAttempt
        {
            LessonId = "leccion-01",
            AttemptedUtc = _clock.UtcNow,
            Score = 75,
            Passed = false,
            DetailsJson = "{\"failedTopics\":[\"TemaB\"]}"
        });

        var data = _dashboard.GetDashboard(LessonCatalog.Instance.Lessons);

        Assert.NotNull(data.LastAttempt);
        Assert.Equal(75, data.LastAttempt!.Score);
        Assert.False(data.LastAttempt.Passed);
        Assert.NotEmpty(data.ReviewTopics);
    }
}
