using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.Services;
using RutaTI30.App.ViewModels;
using RutaTI30.Infrastructure;

namespace RutaTI30.App;

/// <summary>Arranque de la aplicación: base de datos, servicios, ViewModels y ventana principal.</summary>
public partial class App : Application
{
    private ServiceProvider? _services;
    private ILoggerFactory? _loggerFactory;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        try
        {
            var paths = new AppPaths();
            paths.EnsureDirectories();

            var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddProvider(new FileLoggerProvider(paths.LogPath)).SetMinimumLevel(LogLevel.Warning));
            _loggerFactory = loggerFactory;
            var logger = loggerFactory.CreateLogger("App");

            // Inicializar la base de datos y sus migraciones antes de mostrar la ventana.
            var db = new AppDb(paths.DatabasePath);
            db.Initialize();
            logger.LogInformation("Base de datos inicializada en {Path} (esquema v{Version}).",
                paths.DatabasePath, db.GetSchemaVersion());

            var services = new ServiceCollection();
            services.AddSingleton(paths);
            services.AddSingleton(db);
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.AddSingleton<IClock, SystemClock>();

            services.AddSingleton<PreferenceStore>();
            services.AddSingleton<LearningStore>();
            services.AddSingleton<SessionStore>();
            services.AddSingleton<TicketStore>();
            services.AddSingleton<JobStore>();
            services.AddSingleton<LabStore>();
            services.AddSingleton<BackupService>(sp =>
                new BackupService(sp.GetRequiredService<AppDb>(), paths.BackupDirectory));

            services.AddSingleton<ILearningStore>(sp => sp.GetRequiredService<LearningStore>());
            services.AddSingleton<ISessionStore>(sp => sp.GetRequiredService<SessionStore>());
            services.AddSingleton<ITicketStore>(sp => sp.GetRequiredService<TicketStore>());
            services.AddSingleton<IJobStore>(sp => sp.GetRequiredService<JobStore>());
            services.AddSingleton<ILabStore>(sp => sp.GetRequiredService<LabStore>());
            services.AddSingleton<IPreferenceStore>(sp => sp.GetRequiredService<PreferenceStore>());

            services.AddSingleton<ILessonCatalog>(LessonCatalog.Instance);
            services.AddSingleton<ILabCatalog>(LabCatalog.Instance);
            services.AddSingleton<IInterviewBank>(InterviewBank.Instance);

            services.AddSingleton<EvaluationService>(sp =>
                new EvaluationService(GetThreshold(sp.GetRequiredService<IPreferenceStore>())));
            services.AddSingleton<ProgressService>();
            services.AddSingleton<ScheduleService>();
            services.AddSingleton<StudyTimerService>();
            services.AddSingleton<DashboardService>();

            services.AddSingleton<DialogService>();
            services.AddSingleton(sp => new ThemeService(Application.Current));

            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<LearnViewModel>();
            services.AddSingleton<LabViewModel>();
            services.AddSingleton<TicketsViewModel>();
            services.AddSingleton<CareerViewModel>();
            services.AddSingleton<ProgressViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            _services = services.BuildServiceProvider();

            // Recuperar el estado del temporizador (incluye recuperación tras cierre inesperado).
            _services.GetRequiredService<StudyTimerService>().Load();

            var window = _services.GetRequiredService<MainWindow>();
            window.DataContext = _services.GetRequiredService<MainViewModel>();
            MainWindow = window;
            window.Show();

            _ = InitializeDataAsync(logger);
        }
        catch (Exception ex)
        {
            loggerSafe(ex);
            MessageBox.Show(
                "No se pudo iniciar Ruta TI 30.\n\n" + ex.Message +
                "\n\nRevisa la guía de instalación (INSTALLATION.md) y el archivo de registro de errores.",
                "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private async Task InitializeDataAsync(ILogger logger)
    {
        try
        {
            var main = _services!.GetRequiredService<MainViewModel>();
            await main.InitializeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar los datos iniciales.");
            MessageBox.Show(
                "Ocurrió un error al cargar los datos: " + ex.Message,
                "Error de carga", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static double GetThreshold(IPreferenceStore preferences)
    {
        var raw = preferences.GetInt("evaluation.threshold", 80);
        return raw is > 0 and <= 100 ? raw : 80;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            _loggerFactory?.CreateLogger("Dispatcher")
                .LogError(e.Exception, "Excepción no controlada en la interfaz.");
        }
        catch
        {
            // El registro de errores es auxiliar: no debe enmascarar el error original.
        }

        MessageBox.Show(
            "Ocurrió un error inesperado: " + e.Exception.Message +
            "\nLa aplicación continuará; si el problema persiste, revisa el archivo de registro.",
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void loggerSafe(Exception ex)
    {
        try
        {
            _loggerFactory?.CreateLogger("Startup").LogError(ex, "Error fatal al iniciar.");
        }
        catch
        {
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Persistir el estado del temporizador antes de salir (sesiones en curso quedan como "interrumpidas"
        // en el siguiente inicio si corresponde; aquí solo guardamos el tick actual).
        try
        {
            _services?.GetRequiredService<StudyTimerService>().Tick();
        }
        catch
        {
            // Nunca bloquear la salida por un fallo de persistencia del temporizador.
        }

        _services?.Dispose();
        _loggerFactory?.Dispose();
        base.OnExit(e);
    }
}
