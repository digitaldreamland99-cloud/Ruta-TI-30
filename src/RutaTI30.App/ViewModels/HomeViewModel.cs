using System.Collections.ObjectModel;
using System.Windows.Threading;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;
using RutaTI30.Domain;

namespace RutaTI30.App.ViewModels;

/// <summary>Fila del horario mostrada en Inicio.</summary>
public sealed class ScheduleRowVm : ObservableObject
{
    public int Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string TimeRange { get; init; } = string.Empty;
    public string KindName { get; init; } = string.Empty;

    private bool _isNow;
    public bool IsNow
    {
        get => _isNow;
        set => SetProperty(ref _isNow, value);
    }
}

/// <summary>Panel «Mi día»: datos reales del programa, horario y temporizador.</summary>
public sealed class HomeViewModel : ObservableObject, INavigableModule
{
    private readonly DashboardService _dashboard;
    private readonly StudyTimerService _timer;
    private readonly ScheduleService _schedule;
    private readonly ILessonCatalog _catalog;
    private readonly IPreferenceStore _preferences;
    private readonly DialogService _dialogs;
    private readonly IClock _clock;
    private readonly DispatcherTimer _uiTimer;

    public Action<string>? Navigate { get; set; }

    // --- Datos del panel ---
    private string _dayHeader = string.Empty;
    private string _dayTitle = string.Empty;
    private string _dayObjective = string.Empty;
    private string _dateInfo = string.Empty;
    private string _stateBadge = string.Empty;
    private string _studyToday = "0 min";
    private string _studyTotal = "0 min";
    private string _countsText = string.Empty;
    private string _lastAttemptText = "Aún no hay evaluaciones registradas.";
    private string _weeklySummary = string.Empty;
    private string _nextActionText = string.Empty;
    private bool _hasData;

    public string DayHeader { get => _dayHeader; private set => SetProperty(ref _dayHeader, value); }
    public string DayTitle { get => _dayTitle; private set => SetProperty(ref _dayTitle, value); }
    public string DayObjective { get => _dayObjective; private set => SetProperty(ref _dayObjective, value); }
    public string DateInfo { get => _dateInfo; private set => SetProperty(ref _dateInfo, value); }
    public string StateBadge { get => _stateBadge; private set => SetProperty(ref _stateBadge, value); }
    public string StudyToday { get => _studyToday; private set => SetProperty(ref _studyToday, value); }
    public string StudyTotal { get => _studyTotal; private set => SetProperty(ref _studyTotal, value); }
    public string CountsText { get => _countsText; private set => SetProperty(ref _countsText, value); }
    public string LastAttemptText { get => _lastAttemptText; private set => SetProperty(ref _lastAttemptText, value); }
    public string WeeklySummary { get => _weeklySummary; private set => SetProperty(ref _weeklySummary, value); }
    public string NextActionText { get => _nextActionText; private set => SetProperty(ref _nextActionText, value); }
    public bool HasData { get => _hasData; private set { if (SetProperty(ref _hasData, value)) OnPropertyChanged(nameof(IsWelcome)); } }
    public bool IsWelcome => !HasData;

    public ObservableCollection<string> PendingActivities { get; } = new();
    public ObservableCollection<string> ReviewTopics { get; } = new();
    public ObservableCollection<ScheduleRowVm> Schedule { get; } = new();

    // --- Temporizador ---
    private string _timerRemaining = "--:--";
    private string _timerElapsed = "00:00";
    private string _timerStatusText = "Detenido";
    private string _timerBlock = "Sin sesión activa";
    private string _recoveryNote = string.Empty;

    public string TimerRemaining { get => _timerRemaining; private set => SetProperty(ref _timerRemaining, value); }
    public string TimerElapsed { get => _timerElapsed; private set => SetProperty(ref _timerElapsed, value); }
    public string TimerStatusText { get => _timerStatusText; private set => SetProperty(ref _timerStatusText, value); }
    public string TimerBlock { get => _timerBlock; private set => SetProperty(ref _timerBlock, value); }
    public string RecoveryNote { get => _recoveryNote; private set => SetProperty(ref _recoveryNote, value); }

    public AsyncRelayCommand ContinueCommand { get; }
    public AsyncRelayCommand StartCommand { get; }
    public RelayCommand PauseCommand { get; }
    public RelayCommand ResumeCommand { get; }
    public AsyncRelayCommand FinishCommand { get; }
    public RelayCommand ResetCommand { get; }

    public HomeViewModel(DashboardService dashboard, StudyTimerService timer, ScheduleService schedule,
        ILessonCatalog catalog, IPreferenceStore preferences, DialogService dialogs, IClock clock)
    {
        _dashboard = dashboard;
        _timer = timer;
        _schedule = schedule;
        _catalog = catalog;
        _preferences = preferences;
        _dialogs = dialogs;
        _clock = clock;

        ContinueCommand = new AsyncRelayCommand(() =>
        {
            Navigate?.Invoke("learn");
            return Task.CompletedTask;
        });
        StartCommand = new AsyncRelayCommand(StartAsync);
        PauseCommand = new RelayCommand(Pause, () => _timer.State.Status == TimerStatus.EnCurso);
        ResumeCommand = new RelayCommand(Resume, () => _timer.State.Status == TimerStatus.Pausado);
        FinishCommand = new AsyncRelayCommand(FinishAsync);
        ResetCommand = new RelayCommand(Reset, () => _timer.State.Status is TimerStatus.Pausado or TimerStatus.Interrumpido);

        _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _uiTimer.Tick += (_, _) => OnUiTick();
        _uiTimer.Start();

        RebuildSchedule();
        RefreshTimerDisplay();
    }

    public Task RefreshAsync()
    {
        try
        {
            var data = _dashboard.GetDashboard(_catalog.Lessons);
            HasData = data.HasAnyData;
            DayHeader = $"Día {data.CurrentDay} de 30";
            DayTitle = data.DayTitle;
            DayObjective = data.DayObjective;
            DateInfo = $"Fecha de hoy: {data.LocalDate:d} · El día académico no cambia solo por la fecha";
            StateBadge = data.CurrentLessonState switch
            {
                LessonState.NotStarted => "No iniciado",
                LessonState.InProgress => "En curso",
                LessonState.Completed => "Completado",
                LessonState.Mastered => "Dominado",
                _ => ""
            };
            StudyToday = FormatMinutes(data.StudySecondsToday);
            StudyTotal = FormatMinutes(data.StudySecondsTotal);
            CountsText = $"{data.StartedCount} iniciadas · {data.CompletedCount} completadas · {data.MasteredCount} dominadas de {data.TotalLessons}";
            WeeklySummary = $"{data.SessionsLast7Days} sesiones en los últimos 7 días · {data.ActiveDaysLast7Days} días con actividad";

            LastAttemptText = data.LastAttempt is null
                ? "Aún no hay evaluaciones registradas."
                : $"Último resultado: {data.LastAttempt.Score:0.#} % · " +
                  (data.LastAttempt.Passed ? "aprobado" : "no aprobado") +
                  $" · {data.LastAttempt.AttemptedUtc.ToLocalTime():dd/MM/yyyy HH:mm}";

            NextActionText = data.PendingActivities.FirstOrDefault() ?? "Continúa con tu lección del día.";

            PendingActivities.Clear();
            foreach (var activity in data.PendingActivities)
                PendingActivities.Add(activity);

            ReviewTopics.Clear();
            foreach (var topic in data.ReviewTopics)
                ReviewTopics.Add(topic);
            if (ReviewTopics.Count == 0)
                ReviewTopics.Add("Sin temas en revisión: presenta evaluaciones para detectar áreas de mejora.");

            RebuildSchedule();
            RefreshTimerDisplay();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Inicio", "No se pudo cargar el panel: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private void RebuildSchedule()
    {
        Schedule.Clear();
        var now = _clock.LocalNow;
        foreach (var block in _schedule.GetSchedule())
        {
            Schedule.Add(new ScheduleRowVm
            {
                Id = block.Id,
                Label = block.Label,
                TimeRange = $"{ScheduleBlock.FormatMinutes(block.StartMinutes)}–{ScheduleBlock.FormatMinutes(block.EndMinutes)}",
                KindName = block.Kind.ToString(),
                IsNow = now.Hour * 60 + now.Minute >= block.StartMinutes &&
                        now.Hour * 60 + now.Minute < block.EndMinutes
            });
        }
    }

    private Task StartAsync()
    {
        try
        {
            int minutes = Math.Clamp(_preferences.GetInt("timer.default_minutes", 60), 5, 240);
            var kind = _schedule.BlockAt(_clock.LocalNow)?.Kind ?? ScheduleKind.Aprendizaje;
            _timer.Start(minutes * 60, kind);
            RecoveryNote = string.Empty;
            RefreshTimerDisplay();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Temporizador", ex.Message);
        }
        return Task.CompletedTask;
    }

    private void Pause()
    {
        try
        {
            _timer.Pause();
            RefreshTimerDisplay();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Temporizador", ex.Message);
        }
    }

    private void Resume()
    {
        try
        {
            _timer.Resume();
            RefreshTimerDisplay();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Temporizador", ex.Message);
        }
    }

    private Task FinishAsync()
    {
        try
        {
            var session = _timer.Finish();
            _dialogs.Info("Sesión guardada",
                $"Sesión registrada: {StudyTimerService.Format(session.EffectiveSeconds)} de tiempo efectivo " +
                $"(planificado: {StudyTimerService.Format(session.PlannedSeconds)}).");
            RefreshTimerDisplay();
            _ = RefreshAsync();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Temporizador", ex.Message);
        }
        return Task.CompletedTask;
    }

    private void Reset()
    {
        if (!_dialogs.Confirm("Reiniciar temporizador",
                "Se cancelará la sesión actual sin guardar su tiempo. ¿Deseas continuar?"))
            return;
        try
        {
            _timer.Reset();
            RefreshTimerDisplay();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Temporizador", ex.Message);
        }
    }

    private void OnUiTick()
    {
        try
        {
            _timer.Tick();
            RefreshTimerDisplay();

            // Resaltar el bloque activo del horario.
            int minutes = _clock.LocalNow.Hour * 60 + _clock.LocalNow.Minute;
            var blocks = _schedule.GetSchedule();
            foreach (var row in Schedule)
            {
                var block = blocks.FirstOrDefault(b => b.Id == row.Id);
                if (block is not null)
                    row.IsNow = minutes >= block.StartMinutes && minutes < block.EndMinutes;
            }
        }
        catch
        {
            // Un fallo del refresco visual no debe tumbar la aplicación; los datos persisten por separado.
        }
    }

    private void RefreshTimerDisplay()
    {
        var status = _timer.State.Status;
        TimerStatusText = status switch
        {
            TimerStatus.EnCurso => "En curso",
            TimerStatus.Pausado => "En pausa",
            TimerStatus.Interrumpido => "Interrumpida",
            _ => "Detenido"
        };
        TimerElapsed = StudyTimerService.Format(_timer.ElapsedSeconds());
        TimerRemaining = status == TimerStatus.Detenido
            ? "--:--"
            : StudyTimerService.Format((int)_timer.Remaining().TotalSeconds);

        var kind = _timer.State.SessionId is > 0 ? _timer.CurrentBlockKind : (ScheduleKind?)null;
        TimerBlock = kind is null
            ? (_timer.State.Note.Length > 0 ? _timer.State.Note : "Sin sesión activa")
            : $"Bloque: {kind}";
        RecoveryNote = _timer.RecoveryNote;

        StartCommand.RaiseCanExecuteChanged();
        PauseCommand.RaiseCanExecuteChanged();
        ResumeCommand.RaiseCanExecuteChanged();
        FinishCommand.RaiseCanExecuteChanged();
        ResetCommand.RaiseCanExecuteChanged();
    }

    private static string FormatMinutes(int seconds)
    {
        if (seconds <= 0) return "0 min";
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.TotalHours >= 1
            ? $"{(int)ts.TotalHours} h {ts.Minutes:00} min"
            : $"{ts.Minutes} min";
    }
}
