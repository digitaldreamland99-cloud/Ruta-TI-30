using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Application.Services;

/// <summary>
/// Temporizador de estudio basado en marcas de tiempo reales (IClock), no en contadores de tick visual.
/// Reglas: el tiempo en pausa no cuenta; el tiempo con la aplicación cerrada no cuenta; tras un cierre
/// inesperado solo se contabiliza el tiempo verificado hasta el último guardado persistido.
/// </summary>
public sealed class StudyTimerService
{
    public const int PersistIntervalSeconds = 30;

    private readonly ISessionStore _store;
    private readonly IClock _clock;
    private int _plannedSeconds;
    private ScheduleKind _blockKind;
    private bool _loaded;

    public TimerStateRow State { get; private set; } = new();
    /// <summary>Explicación legible de la recuperación (p. ej. cierre inesperado), vacía si no aplica.</summary>
    public string RecoveryNote { get; private set; } = string.Empty;

    public StudyTimerService(ISessionStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    /// <summary>Carga el estado persistido. Si había una sesión "en curso" (cierre inesperado),
    /// la cierra contando solo el tiempo verificado hasta el último tick persistido.</summary>
    public void Load()
    {
        State = _store.GetTimerState();
        if (State.Status == TimerStatus.EnCurso)
        {
            var now = _clock.UtcNow;
            var lastResume = State.LastResumeUtc ?? now;
            var verifiedTick = State.PersistedTickUtc ?? lastResume;
            int runningPart = verifiedTick > lastResume ? (int)(verifiedTick - lastResume).TotalSeconds : 0;
            int effective = State.AccumulatedSeconds + Math.Max(0, runningPart);

            if (State.SessionId is long sessionId)
            {
                var session = _store.GetSession(sessionId);
                if (session is not null)
                {
                    session.EndedUtc = verifiedTick;
                    session.EffectiveSeconds = effective;
                    session.Note = "Sesión interrumpida al cerrar la aplicación. Se contó el tiempo verificado " +
                                   $"hasta {verifiedTick.ToLocalTime():HH:mm:ss}; el tiempo no verificado no se cuenta.";
                    _store.SaveSession(session);
                }
            }

            State = new TimerStateRow
            {
                Status = TimerStatus.Interrumpido,
                AccumulatedSeconds = 0,
                PersistedTickUtc = verifiedTick,
                Note = $"Interrumpida. Tiempo verificado conservado en la sesión: {Format(effective)}."
            };
            _store.SaveTimerState(State);
            RecoveryNote = $"Se recuperó una sesión interrumpida: se conservaron {Format(effective)} " +
                           "de tiempo verificado. El tiempo transcurrido con la aplicación cerrada no se contó.";
        }
        else
        {
            RecoveryNote = State.Status == TimerStatus.Pausado
                ? "Había una sesión en pausa; reanúdala cuando quieras continuar."
                : string.Empty;
        }
        _loaded = true;
    }

    public void Start(int plannedSeconds, ScheduleKind kind)
    {
        EnsureLoaded();
        if (State.Status == TimerStatus.EnCurso)
            throw new InvalidOperationException("Ya hay una sesión en curso. Finaliza o pausa la sesión actual.");
        if (plannedSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(plannedSeconds), "La duración planificada debe ser positiva.");

        var now = _clock.UtcNow;
        var session = new StudySession
        {
            StartedUtc = now,
            PlannedSeconds = plannedSeconds,
            BlockKind = kind
        };
        _store.SaveSession(session);

        _plannedSeconds = plannedSeconds;
        _blockKind = kind;
        State = new TimerStateRow
        {
            Status = TimerStatus.EnCurso,
            SessionId = session.Id,
            StartedUtc = now,
            LastResumeUtc = now,
            AccumulatedSeconds = 0,
            PersistedTickUtc = now
        };
        _store.SaveTimerState(State);
        RecoveryNote = string.Empty;
    }

    public void Pause()
    {
        EnsureLoaded();
        if (State.Status != TimerStatus.EnCurso)
            throw new InvalidOperationException("Solo se puede pausar una sesión en curso.");

        var now = _clock.UtcNow;
        State.AccumulatedSeconds += RunningSeconds(now);
        State.LastResumeUtc = null;
        State.PersistedTickUtc = now;
        State.Status = TimerStatus.Pausado;
        _store.SaveTimerState(State);
    }

    public void Resume()
    {
        EnsureLoaded();
        if (State.Status != TimerStatus.Pausado)
            throw new InvalidOperationException("Solo se puede reanudar una sesión pausada.");

        var now = _clock.UtcNow;
        State.LastResumeUtc = now;
        State.PersistedTickUtc = now;
        State.Status = TimerStatus.EnCurso;
        _store.SaveTimerState(State);
    }

    /// <summary>Finaliza la sesión: guarda tiempo efectivo verificado y libera el estado.</summary>
    public StudySession Finish()
    {
        EnsureLoaded();
        if (State.Status is not (TimerStatus.EnCurso or TimerStatus.Pausado))
            throw new InvalidOperationException("No hay una sesión activa para finalizar.");

        var now = _clock.UtcNow;
        int effective = State.AccumulatedSeconds +
                        (State.Status == TimerStatus.EnCurso ? RunningSeconds(now) : 0);

        var session = _store.GetSession(State.SessionId ?? 0)
            ?? throw new InvalidOperationException("La sesión persistida del temporizador no se encontró.");
        session.EndedUtc = now;
        session.EffectiveSeconds = Math.Max(0, effective);
        session.Note = string.IsNullOrEmpty(session.Note) ? "Finalizada normalmente." : session.Note;
        _store.SaveSession(session);

        State = new TimerStateRow { Status = TimerStatus.Detenido, Note = string.Empty };
        _store.SaveTimerState(State);
        RecoveryNote = string.Empty;
        return session;
    }

    /// <summary>Reinicia el estado detenido (la interfaz exige confirmación previa). No borra sesiones guardadas.</summary>
    public void Reset()
    {
        EnsureLoaded();
        if (State.Status == TimerStatus.EnCurso)
            throw new InvalidOperationException("Finaliza o pausa la sesión antes de reiniciar.");
        State = new TimerStateRow { Status = TimerStatus.Detenido, Note = string.Empty };
        _store.SaveTimerState(State);
        RecoveryNote = string.Empty;
    }

    /// <summary>Segundos transcurridos de la sesión actual (acumulado verificado + período en curso).</summary>
    public int ElapsedSeconds()
    {
        EnsureLoaded();
        var now = _clock.UtcNow;
        return State.Status == TimerStatus.EnCurso
            ? State.AccumulatedSeconds + RunningSeconds(now)
            : State.AccumulatedSeconds;
    }

    /// <summary>Tiempo restante del bloque planificado (nunca negativo).</summary>
    public TimeSpan Remaining()
    {
        EnsureLoaded();
        int planned = _plannedSeconds > 0 ? _plannedSeconds : CurrentSessionPlanned();
        int remaining = Math.Max(0, planned - ElapsedSeconds());
        return TimeSpan.FromSeconds(remaining);
    }

    /// <summary>Llamar desde la UI (1 s) solo para refrescar visualización y persistir el tick si corresponde.</summary>
    public void Tick()
    {
        EnsureLoaded();
        if (State.Status != TimerStatus.EnCurso) return;
        var now = _clock.UtcNow;
        if (State.PersistedTickUtc is null || (now - State.PersistedTickUtc.Value).TotalSeconds >= PersistIntervalSeconds)
        {
            State.PersistedTickUtc = now;
            _store.SaveTimerState(State);
        }
    }

    public ScheduleKind CurrentBlockKind => State.SessionId is long id && id > 0
        ? (_store.GetSession(id)?.BlockKind ?? _blockKind)
        : _blockKind;

    private int RunningSeconds(DateTime now)
    {
        if (State.LastResumeUtc is null) return 0;
        return Math.Max(0, (int)(now - State.LastResumeUtc.Value).TotalSeconds);
    }

    private int CurrentSessionPlanned() =>
        State.SessionId is long id && id > 0 ? _store.GetSession(id)?.PlannedSeconds ?? 0 : 0;

    private void EnsureLoaded()
    {
        if (!_loaded)
            throw new InvalidOperationException("Llama a Load() antes de usar el temporizador.");
    }

    public static string Format(int totalSeconds)
    {
        var ts = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
        return ts.TotalHours >= 1 ? $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}" : $"{ts.Minutes:00}:{ts.Seconds:00}";
    }
}
