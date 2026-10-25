using RutaTI30.App.Mvvm;
using RutaTI30.Application.Abstractions;

namespace RutaTI30.App.ViewModels;

/// <summary>Elemento de la navegación lateral.</summary>
public sealed record NavItem(string Key, string Label);

/// <summary>Módulo que carga sus datos al navegar hacia él.</summary>
public interface INavigableModule
{
    Task RefreshAsync();
}

/// <summary>VM principal: navega entre los nueve módulos y actualiza el encabezado.</summary>
public sealed class MainViewModel : ObservableObject
{
    private readonly HomeViewModel _home;
    private readonly LearnViewModel _learn;
    private readonly LabViewModel _lab;
    private readonly TicketsViewModel _tickets;
    private readonly CareerViewModel _career;
    private readonly ProgressViewModel _progress;
    private readonly GlossaryViewModel _glossary;
    private readonly RoutesViewModel _routes;
    private readonly SettingsViewModel _settings;
    private readonly IDbStateChanged _dbChanged;

    private NavItem? _selectedNav;
    private object? _currentViewModel;
    private string _currentTitle = "Inicio";
    private string _currentDate = string.Empty;

    public IReadOnlyList<NavItem> NavItems { get; } = new List<NavItem>
    {
        new("home", "Inicio"),
        new("learn", "Mi ruta de aprendizaje"),
        new("lab", "Laboratorio de soporte"),
        new("tickets", "Simulador de Help Desk"),
        new("career", "Preparación laboral"),
        new("progress", "Mi progreso"),
        new("glossary", "Glosario"),
        new("routes", "Rutas"),
        new("settings", "Configuración")
    };

    public MainViewModel(HomeViewModel home, LearnViewModel learn, LabViewModel lab,
        TicketsViewModel tickets, CareerViewModel career, ProgressViewModel progress,
        GlossaryViewModel glossary, RoutesViewModel routes, SettingsViewModel settings,
        IDbStateChanged dbChanged)
    {
        _home = home;
        _learn = learn;
        _lab = lab;
        _tickets = tickets;
        _career = career;
        _progress = progress;
        _glossary = glossary;
        _routes = routes;
        _settings = settings;
        _dbChanged = dbChanged;

        // Cuando la base de datos cambia (p. ej. restauración de una copia), refrescar todos los
        // módulos con estado cargado para que ningún panel siga mostrando datos de la base anterior.
        _dbChanged.DataChanged += OnDatabaseChanged;
        // Enlaces rápidos desde Inicio hacia otros módulos.
        _home.Navigate = key => SelectedNav = NavItems.FirstOrDefault(n => n.Key == key) ?? NavItems[0];

        _selectedNav = NavItems[0];
        _currentViewModel = _home;
        _currentTitle = NavItems[0].Label;
        _currentDate = DateTime.Now.ToString("dddd, d 'de' MMMM 'de' yyyy");
    }

    public NavItem? SelectedNav
    {
        get => _selectedNav;
        set
        {
            if (!SetProperty(ref _selectedNav, value) || value is null) return;
            _currentTitle = value.Label;
            OnPropertyChanged(nameof(CurrentTitle));
            _currentViewModel = value.Key switch
            {
                "learn" => _learn,
                "lab" => _lab,
                "tickets" => _tickets,
                "career" => _career,
                "progress" => _progress,
                "glossary" => _glossary,
                "routes" => _routes,
                "settings" => _settings,
                _ => _home
            };
            OnPropertyChanged(nameof(CurrentViewModel));
            _ = RefreshCurrentAsync();
        }
    }

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public string CurrentTitle
    {
        get => _currentTitle;
        private set => SetProperty(ref _currentTitle, value);
    }

    public string CurrentDate
    {
        get => _currentDate;
        private set => SetProperty(ref _currentDate, value);
    }

    /// <summary>Carga inicial (llamada al arrancar la aplicación).</summary>
    public Task InitializeAsync() => RefreshCurrentAsync();

    private void OnDatabaseChanged()
    {
        _ = RefreshCurrentAsync();
        _ = _home.RefreshAsync();
        _ = _progress.RefreshAsync();
        _ = _learn.RefreshAsync();
        _ = _lab.RefreshAsync();
        _ = _tickets.RefreshAsync();
        _ = _career.RefreshAsync();
        _ = _settings.RefreshAsync();
        // El glosario y las rutas provienen de catálogos en memoria, no de la base: no requieren refresco.
    }

    private async Task RefreshCurrentAsync()
    {
        try
        {
            if (_currentViewModel is INavigableModule module)
                await module.RefreshAsync();
        }
        catch (Exception ex)
        {
            // Los errores de carga se muestran en el propio módulo cuando ocurren allí;
            // aquí solo evitamos que la excepción quede sin observar.
            System.Diagnostics.Debug.WriteLine($"Error al refrescar módulo: {ex.Message}");
        }
    }
}
