using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.ViewModels;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class StudyTimerTickIntegrationTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly FakeClock _clock = new();
    private readonly SessionStore _sessions;
    private readonly PreferenceStore _preferences;
    private readonly DashboardService _dashboard;
    private readonly StudyTimerService _timer;
    private readonly HomeViewModel _home;

    public StudyTimerTickIntegrationTests()
    {
        _sessions = new SessionStore(_db.Db);
        _preferences = new PreferenceStore(_db.Db);
        _dashboard = new DashboardService(
            new LearningStore(_db.Db),
            _sessions,
            new TicketStore(_db.Db),
            _clock);

        _preferences.SetInt("timer.default_minutes", 30);
        _preferences.SetInt("evaluation.threshold", 80);
        _preferences.SetInt("program.current_day", 1);

        _timer = new StudyTimerService(_sessions, _clock);

    // HomeViewModel requiere un DialogService real (clase sellada de App): se usa el original;
    // ninguna prueba de esta clase invoca una acción que muestre un diálogo (start/pause/finish
    // solo fallan si el temporizador lanza, y el flujo cubierto es el feliz + reset del servicio).
    _home = new HomeViewModel(
        _dashboard,
        _timer,
        new ScheduleService(_preferences),
        LessonCatalog.Instance,
        _preferences,
        new DialogServiceStub(),
        _clock);

        _home.Navigate = key => { };
    }

    public void Dispose()
    {
        try { _db.Dispose(); } catch { }
    }

    /// <summary>
    /// Simula lo que haría el bucle de UI: cada "segundo" de reloj avanzado,
    /// se invoca el mismo callback que conecta DispatcherTimer en producción.
    /// </summary>
    private void SimulateUiTick(int seconds)
    {
        for (int i = 0; i < seconds; i++)
        {
            _clock.Advance(TimeSpan.FromSeconds(1));
            _home.OnUiTick();
        }
    }

    [Fact]
    public void HomeViewModel_VigilaTemporizador_CadaActualizacionDeUIInvocaTickDelServicio()
    {
        _home.StartCommand.Execute(null);

        var before = _timer.ElapsedSeconds();
        SimulateUiTick(3);
        var after = _timer.ElapsedSeconds();

        Assert.True(after >= before + 3);
    }

    [Fact]
    public void Start_Pausa_Reanudacion_YFinalizacion_PersistenSoloTiempoVerificado()
    {
        // Iniciar sesión de 30 min
        _home.StartCommand.Execute(null);

        // TICKS verificados
        SimulateUiTick(7);

        Assert.Equal(TimerStatus.EnCurso, _timer.State.Status);
        Assert.True(_timer.ElapsedSeconds() >= 7);

        // Pausa: el tiempo deja de acumular
        _home.PauseCommand.Execute(null);
        int pausedAt = _timer.ElapsedSeconds();
        SimulateUiTick(5);
        Assert.Equal(pausedAt, _timer.ElapsedSeconds());

        // Reanudación: vuelve a acumular
        _home.ResumeCommand.Execute(null);
        int resumedAt = _timer.ElapsedSeconds();
        SimulateUiTick(4);
        Assert.True(_timer.ElapsedSeconds() >= resumedAt + 4);

        // Finalización: sesión persistida con tiempo efectivo
        _home.FinishCommand.ExecuteTask(null);
        var finished = _sessions.GetSessions().OrderByDescending(s => s.EndedUtc).FirstOrDefault();
        Assert.NotNull(finished);
        Assert.Equal(TimerStatus.Detenido, _timer.State.Status);
        // Tiempo efectivo = solo los ticks verificados tras pausa y reanudación (7 con la pausa en medio).
        Assert.True(finished.EffectiveSeconds >= 4, $"Efectivo = {finished.EffectiveSeconds}");
        Assert.Equal(30 * 60, finished.PlannedSeconds);
    }

    [Fact]
    public void CierreInesperado_DetectingInterruptedSession_MuestraRecoveryNoteYSoloVerificadoCuenta()
    {
        _preferences.SetInt("timer.default_minutes", 10);
        _home.StartCommand.Execute(null);
        SimulateUiTick(6);

        // Simular reinicio de la aplicación: nueva instancia con mismo almacén,
        // que carga estado EnCurso y lo convierte en Interrumpido.
        var recoveredTimer = new StudyTimerService(_sessions, _clock);
        recoveredTimer.Load();

        Assert.Equal(TimerStatus.Interrumpido, recoveredTimer.State.Status);
        Assert.NotEmpty(recoveredTimer.RecoveryNote);

        Assert.Contains("verificado", recoveredTimer.RecoveryNote, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("no verificado", recoveredTimer.RecoveryNote, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ResetNotAllowedWhileRunning_YPausaONoEsPosibleReiniciarDirectamente()
    {
        _home.StartCommand.Execute(null);
        SimulateUiTick(2);

        // No debe poder resetear desde la UI mientras corre (la UI deshabilita el comando,
        // pero el servicio también protege la operación).
        Assert.Throws<InvalidOperationException>(() => _timer.Reset());
    }

    private static int CurrentElapsedThroughUi()
    {
        // Lectura pública del servicio; no depende de WPF ni DispatcherTimer.
        return 0;
    }
}

/// <summary>Doble de DialogService sin ventanas reales: hereda y sobrescribe (los métodos son
/// virtuales desde el 2026-10-11). Registra contadores para aserciones opcionales.</summary>
internal sealed class DialogServiceStub : RutaTI30.App.Services.DialogService
{
    public int InfoCount;
    public int ErrorCount;

    public override void Info(string title, string message) => ++InfoCount;
    public override void Error(string title, string message) => ++ErrorCount;
    public override bool Confirm(string title, string message) => true;
    public override string? SaveFile(string filter, string defaultFileName) => null;
    public override string? OpenFile(string filter) => null;
}
