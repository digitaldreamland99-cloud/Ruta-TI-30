using System.Collections.ObjectModel;
using System.Reflection;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Services;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;

namespace RutaTI30.App.ViewModels;

/// <summary>Fila editable del horario.</summary>
public sealed class ScheduleBlockVm : ObservableObject
{
    public int Id { get; init; }
    private string _label = string.Empty;
    private string _start = string.Empty;
    private string _end = string.Empty;

    public string Label { get => _label; set => SetProperty(ref _label, value); }
    public string Start { get => _start; set => SetProperty(ref _start, value); }
    public string End { get => _end; set => SetProperty(ref _end, value); }
    public string KindName { get; init; } = string.Empty;
    public ScheduleKind Kind { get; init; }
}

/// <summary>Configuración: tema, horario, preferencias, datos y copias de seguridad.</summary>
public sealed class SettingsViewModel : ObservableObject, INavigableModule
{
    private readonly ScheduleService _schedule;
    private readonly DashboardService _dashboard;
    private readonly IPreferenceStore _preferences;
    private readonly BackupService _backup;
    private readonly AppPaths _paths;
    private readonly AppDb _db;
    private readonly ThemeService _theme;
    private readonly DialogService _dialogs;

    // Tema
    public IReadOnlyList<string> ThemeOptions { get; } = new[] { "Oscuro", "Claro" };
    private string _selectedTheme = "Oscuro";
    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (!SetProperty(ref _selectedTheme, value)) return;
            _theme.Apply(value == "Claro" ? ThemeService.Light : ThemeService.Dark);
            _preferences.Set("theme", value == "Claro" ? ThemeService.Light : ThemeService.Dark);
        }
    }

    // Preferencias numéricas
    private string _sessionMinutes = "60";
    private string _threshold = "80";
    private string _currentDay = "1";
    private string _settingsMessage = string.Empty;

    public string SessionMinutes { get => _sessionMinutes; set => SetProperty(ref _sessionMinutes, value); }
    public string Threshold { get => _threshold; set => SetProperty(ref _threshold, value); }
    public string CurrentDay { get => _currentDay; set => SetProperty(ref _currentDay, value); }
    public string SettingsMessage { get => _settingsMessage; private set => SetProperty(ref _settingsMessage, value); }

    // Horario
    public ObservableCollection<ScheduleBlockVm> Blocks { get; } = new();

    // Datos
    public string DataPath => _paths.DatabasePath;
    public string VersionText { get; }

    public AsyncRelayCommand SavePreferencesCommand { get; }
    public AsyncRelayCommand SaveScheduleCommand { get; }
    public AsyncRelayCommand AddBlockCommand { get; }
    public AsyncRelayCommand CreateBackupCommand { get; }
    public AsyncRelayCommand RestoreBackupCommand { get; }

    public SettingsViewModel(ScheduleService schedule, DashboardService dashboard, IPreferenceStore preferences,
        BackupService backup, AppPaths paths, AppDb db, ThemeService theme, DialogService dialogs)
    {
        _schedule = schedule;
        _dashboard = dashboard;
        _preferences = preferences;
        _backup = backup;
        _paths = paths;
        _db = db;
        _theme = theme;
        _dialogs = dialogs;

        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.1.0";
        VersionText = $"Ruta TI 30 v{version} · esquema de datos v{_db.GetSchemaVersion()}";

        SavePreferencesCommand = new AsyncRelayCommand(SavePreferencesAsync);
        SaveScheduleCommand = new AsyncRelayCommand(SaveScheduleAsync);
        AddBlockCommand = new AsyncRelayCommand(AddBlockAsync);
        CreateBackupCommand = new AsyncRelayCommand(CreateBackupAsync);
        RestoreBackupCommand = new AsyncRelayCommand(RestoreBackupAsync);
    }

    public Task RefreshAsync()
    {
        try
        {
            var theme = _preferences.Get("theme") ?? ThemeService.Dark;
            _selectedTheme = theme == ThemeService.Light ? "Claro" : "Oscuro";
            OnPropertyChanged(nameof(SelectedTheme));

            SessionMinutes = _preferences.GetInt("timer.default_minutes", 60).ToString();
            Threshold = _preferences.GetInt("evaluation.threshold", 80).ToString();
            CurrentDay = _dashboard.GetCurrentDay().ToString();

            Blocks.Clear();
            foreach (var b in _schedule.GetSchedule())
            {
                Blocks.Add(new ScheduleBlockVm
                {
                    Id = b.Id,
                    Label = b.Label,
                    Start = ScheduleBlock.FormatMinutes(b.StartMinutes),
                    End = ScheduleBlock.FormatMinutes(b.EndMinutes),
                    Kind = b.Kind,
                    KindName = b.Kind.ToString()
                });
            }
            SettingsMessage = string.Empty;
        }
        catch (Exception ex)
        {
            _dialogs.Error("Configuración", "No se pudo cargar: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private Task SavePreferencesAsync()
    {
        try
        {
            if (!int.TryParse(SessionMinutes, out var minutes) || minutes is < 5 or > 240)
            {
                SettingsMessage = "La duración de sesión debe estar entre 5 y 240 minutos.";
                return Task.CompletedTask;
            }
            if (!int.TryParse(Threshold, out var threshold) || threshold is < 1 or > 100)
            {
                SettingsMessage = "El umbral de aprobación debe estar entre 1 y 100.";
                return Task.CompletedTask;
            }
            if (!int.TryParse(CurrentDay, out var day) || day is < 1 or > 30)
            {
                SettingsMessage = "El día del programa debe estar entre 1 y 30.";
                return Task.CompletedTask;
            }

            _preferences.SetInt("timer.default_minutes", minutes);
            _preferences.SetInt("evaluation.threshold", threshold);
            _dashboard.SetCurrentDay(day);
            SettingsMessage = "Preferencias guardadas. El umbral aplica en las nuevas evaluaciones.";
        }
        catch (Exception ex)
        {
            _dialogs.Error("Configuración", "No se pudieron guardar las preferencias: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private async Task SaveScheduleAsync()
    {
        try
        {
            var blocks = new List<ScheduleBlock>();
            foreach (var row in Blocks)
            {
                if (!TryParseTime(row.Start, out var start) || !TryParseTime(row.End, out var end))
                {
                    SettingsMessage = $"Horario inválido en «{row.Label}»: usa el formato HH:mm.";
                    return;
                }
                blocks.Add(new ScheduleBlock
                {
                    Id = row.Id,
                    StartMinutes = start,
                    EndMinutes = end,
                    Kind = row.Kind,
                    Label = row.Label?.Trim() ?? string.Empty
                });
            }

            var errors = _schedule.SaveSchedule(blocks);
            if (errors.Count > 0)
            {
                SettingsMessage = string.Join(" ", errors);
                return;
            }
            SettingsMessage = "Horario guardado. El contenido siempre es accesible, con o sin horario.";
        }
        catch (Exception ex)
        {
            _dialogs.Error("Horario", "No se pudo guardar: " + ex.Message);
        }
        await Task.CompletedTask;
    }

    private Task AddBlockAsync()
    {
        int nextId = Blocks.Count == 0 ? 1 : Blocks.Max(b => b.Id) + 1;
        Blocks.Add(new ScheduleBlockVm
        {
            Id = nextId,
            Label = "Nuevo bloque",
            Start = "16:00",
            End = "17:00",
            Kind = ScheduleKind.Repaso,
            KindName = ScheduleKind.Repaso.ToString()
        });
        SettingsMessage = "Bloque añadido: edita sus horas y guarda.";
        return Task.CompletedTask;
    }

    private Task CreateBackupAsync()
    {
        try
        {
            var path = _backup.CreateBackup();
            _dialogs.Info("Copia de seguridad",
                "Copia creada:\n" + path + "\n\nGuárdala en un lugar seguro. Ver BACKUP_RESTORE.md.");
        }
        catch (Exception ex)
        {
            _dialogs.Error("Copia de seguridad", "No se pudo crear la copia: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private async Task RestoreBackupAsync()
    {
        try
        {
            var path = _dialogs.OpenFile("Base de datos de Ruta TI 30 (*.db)|*.db");
            if (path is null) return Task.CompletedTask;

            if (!_dialogs.Confirm("Restaurar copia de seguridad",
                    "Se reemplazarán los datos actuales por los de la copia seleccionada.\n" +
                    "Se conservará una copia automática del estado actual.\n\n¿Deseas continuar?"))
                return Task.CompletedTask;

            var result = _backup.Restore(path);
            if (result.Success)
            {
                _dialogs.Info("Restauración completada",
                    "Los datos se restauraron correctamente." +
                    (result.SafetyBackupPath is not null
                        ? "\n\nCopia del estado previo:\n" + result.SafetyBackupPath
                        : string.Empty));
                await RefreshAsync();
            }
            else
            {
                _dialogs.Error("Restauración cancelada",
                    "La copia no es válida o la restauración falló:\n- " +
                    string.Join("\n- ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _dialogs.Error("Restauración", "No se pudo restaurar: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private static bool TryParseTime(string? text, out int minutes)
    {
        minutes = 0;
        if (!TimeSpan.TryParse(text, out var time)) return false;
        if (time.TotalMinutes < 0 || time.TotalMinutes > 24 * 60) return false;
        minutes = (int)time.TotalMinutes;
        return true;
    }
}
