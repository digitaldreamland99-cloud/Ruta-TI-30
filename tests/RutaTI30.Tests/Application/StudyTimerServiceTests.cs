using RutaTI30.Application.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class StudyTimerServiceTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly FakeClock _clock = new();
    private readonly SessionStore _store;

    public StudyTimerServiceTests()
    {
        _store = new SessionStore(_db.Db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public void Start_CalculaTiempoPorMarcasDeTiempo()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(60 * 60, ScheduleKind.Aprendizaje);

        _clock.AdvanceMinutes(10);
        Assert.Equal(600, timer.ElapsedSeconds());
        Assert.Equal(TimeSpan.FromMinutes(50), timer.Remaining());
    }

    [Fact]
    public void Pause_ElTiempoNoSeCuenta()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(60 * 60, ScheduleKind.Aprendizaje);

        _clock.AdvanceMinutes(10);
        timer.Pause();
        _clock.AdvanceMinutes(300); // tiempo en pausa / app cerrada

        Assert.Equal(600, timer.ElapsedSeconds());
        Assert.Equal(TimerStatus.Pausado, timer.State.Status);
    }

    [Fact]
    public void Resume_AcumulaSoloElTiempoEnCurso()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(60 * 60, ScheduleKind.PracticaGuiada);

        _clock.AdvanceMinutes(10);
        timer.Pause();
        _clock.AdvanceMinutes(120);
        timer.Resume();
        _clock.AdvanceMinutes(5);

        Assert.Equal(900, timer.ElapsedSeconds()); // 10 + 5, sin la pausa de 120
    }

    [Fact]
    public void Finish_GuardaSesionConTiempoEfectivo()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(3600, ScheduleKind.Laboratorio);
        _clock.AdvanceMinutes(42);

        var session = timer.Finish();

        Assert.Equal(42 * 60, session.EffectiveSeconds);
        Assert.Equal(3600, session.PlannedSeconds);
        Assert.NotNull(session.EndedUtc);
        Assert.Equal(TimerStatus.Detenido, _store.GetTimerState().Status);

        var stored = _store.GetSession(session.Id);
        Assert.NotNull(stored);
        Assert.Equal(42 * 60, stored!.EffectiveSeconds);
    }

    [Fact]
    public void Recuperacion_CierreInesperado_NoInflaElTiempo()
    {
        // Sesión en curso con tick persistido hace 10 minutos...
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(7200, ScheduleKind.Aprendizaje);
        _clock.AdvanceMinutes(10);
        timer.Tick(); // persiste el tick

        // ...y la aplicación "se cierra" durante 5 horas (el reloj avanza sin que nadie registre ticks).
        _clock.AdvanceMinutes(300);

        var recuperado = new StudyTimerService(_store, _clock);
        recuperado.Load();

        // Solo se conserva el tiempo verificado hasta el último tick: 10 minutos, no 5 h 10 min.
        Assert.Equal(TimerStatus.Interrumpido, recuperado.State.Status);
        Assert.NotEmpty(recuperado.RecoveryNote);
        // No hay sesión activa tras la recuperación: el temporizador queda detenido (tiempo no inflado).
        Assert.Equal(0, recuperado.ElapsedSeconds());

        var sessions = _store.GetSessions();
        var interrumpida = sessions.First(s => s.Note.Contains("interrumpida", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(600, interrumpida.EffectiveSeconds);
        Assert.Contains("no se cuenta", interrumpida.Note);
    }

    [Fact]
    public void Start_Duplicado_LanzaInvalidOperation()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(600, ScheduleKind.Aprendizaje);

        Assert.Throws<InvalidOperationException>(() => timer.Start(600, ScheduleKind.Aprendizaje));
    }

    [Fact]
    public void Pause_SinSesion_LanzaInvalidOperation()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();

        Assert.Throws<InvalidOperationException>(() => timer.Pause());
    }

    [Fact]
    public void Finish_SinSesion_LanzaInvalidOperation()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();

        Assert.Throws<InvalidOperationException>(() => timer.Finish());
    }

    [Fact]
    public void UseSinLoad_LanzaInvalidOperation()
    {
        var timer = new StudyTimerService(_store, _clock);
        Assert.Throws<InvalidOperationException>(() => timer.ElapsedSeconds());
    }

    [Fact]
    public void EstadoPersistido_SobreviveEntreInstancias()
    {
        var timer = new StudyTimerService(_store, _clock);
        timer.Load();
        timer.Start(3600, ScheduleKind.Repaso);
        timer.Pause();

        var otro = new StudyTimerService(_store, _clock);
        otro.Load();

        Assert.Equal(TimerStatus.Pausado, otro.State.Status);
        Assert.NotEmpty(otro.RecoveryNote); // hay sesión en pausa recuperable
    }
}
