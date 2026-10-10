using System.Collections.ObjectModel;
using System.Text;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;

namespace RutaTI30.App.ViewModels;

/// <summary>Intento reciente en la lista.</summary>
public sealed record AttemptItemVm(string WhenLocal, string Lesson, string Score, string Result);

/// <summary>Progreso calculado solo desde datos persistidos, con informe exportable.</summary>
public sealed class ProgressViewModel : ObservableObject, INavigableModule
{
    private readonly ProgressService _progress;
    private readonly ILessonCatalog _catalog;
    private readonly ILearningStore _learning;
    private readonly DialogService _dialogs;
    private readonly IClock _clock;

    private string _lessonsText = string.Empty;
    private string _evaluationsText = string.Empty;
    private string _studyText = string.Empty;
    private string _ticketsText = string.Empty;
    private string _reportText = string.Empty;
    private bool _hasData;

    public string LessonsText { get => _lessonsText; private set => SetProperty(ref _lessonsText, value); }
    public string EvaluationsText { get => _evaluationsText; private set => SetProperty(ref _evaluationsText, value); }
    public string StudyText { get => _studyText; private set => SetProperty(ref _studyText, value); }
    public string TicketsText { get => _ticketsText; private set => SetProperty(ref _ticketsText, value); }
    public string ReportText { get => _reportText; private set => SetProperty(ref _reportText, value); }
    public bool HasData { get => _hasData; private set { if (SetProperty(ref _hasData, value)) OnPropertyChanged(nameof(NoDataMessage)); } }
    public bool NoDataMessage => !HasData;

    public ObservableCollection<string> WeakTopics { get; } = new();
    public ObservableCollection<string> DemonstratedTopics { get; } = new();
    public ObservableCollection<AttemptItemVm> RecentAttempts { get; } = new();

    public AsyncRelayCommand GenerateReportCommand { get; }
    public AsyncRelayCommand ExportReportCommand { get; }

    public ProgressViewModel(ProgressService progress, ILessonCatalog catalog, ILearningStore learning,
        DialogService dialogs, IClock clock)
    {
        _progress = progress;
        _catalog = catalog;
        _learning = learning;
        _dialogs = dialogs;
        _clock = clock;
        GenerateReportCommand = new AsyncRelayCommand(() => { GenerateReport(); return Task.CompletedTask; });
        ExportReportCommand = new AsyncRelayCommand(ExportReportAsync);
    }

    public Task RefreshAsync()
    {
        try
        {
            var stats = _progress.GetStatistics(_catalog.Lessons);
            HasData = stats.Started > 0 || stats.TotalStudySeconds > 0 || stats.EvaluationsTaken > 0;

            LessonsText =
                $"{stats.Completed} completadas · {stats.Mastered} dominadas · {stats.Started} iniciadas de {stats.TotalLessons}";
            EvaluationsText = stats.AverageScore is double avg
                ? $"{stats.EvaluationsTaken} intentos · promedio {avg:0.#} %"
                : $"{stats.EvaluationsTaken} intentos (aún sin resultados)";
            StudyText = $"{FormatMinutes(stats.TotalStudySeconds)} acumuladas · hoy {FormatMinutes(stats.StudySecondsToday)}";
            TicketsText = $"{stats.TicketsCreated} creados · {stats.TicketsResolved} resueltos/cerrados";

            WeakTopics.Clear();
            foreach (var t in stats.WeakTopics) WeakTopics.Add(t);
            if (WeakTopics.Count == 0)
                WeakTopics.Add("Sin detectados aún: presenta evaluaciones para identificar temas débiles.");

            DemonstratedTopics.Clear();
            foreach (var t in stats.DemonstratedTopics) DemonstratedTopics.Add(t);
            if (DemonstratedTopics.Count == 0)
                DemonstratedTopics.Add("Aún no se han demostrado competencias: aprueba evaluaciones para mostrarlas.");

            RecentAttempts.Clear();
            foreach (var a in _learning.GetAttempts().TakeLast(15).Reverse())
            {
                var lesson = _catalog.GetById(a.LessonId);
                RecentAttempts.Add(new AttemptItemVm(
                    a.AttemptedUtc.ToLocalTime().ToString("dd/MM HH:mm"),
                    lesson is null ? a.LessonId : $"Día {lesson.Day}",
                    $"{a.Score:0.#} %",
                    a.Passed ? "aprobado" : "no aprobado"));
            }
        }
        catch (Exception ex)
        {
            _dialogs.Error("Progreso", "No se pudieron calcular las estadísticas: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private void GenerateReport()
    {
        try
        {
            var stats = _progress.GetStatistics(_catalog.Lessons);
            var sb = new StringBuilder();
            sb.AppendLine("# Informe de progreso — Ruta TI 30");
            sb.AppendLine($"Generado: {_clock.LocalNow:yyyy-MM-dd HH:mm}");
            sb.AppendLine();
            sb.AppendLine("## Actividades realizadas");
            sb.AppendLine($"- Lecciones iniciadas: {stats.Started} de {stats.TotalLessons}");
            sb.AppendLine($"- Lecciones completadas: {stats.Completed}");
            sb.AppendLine($"- Lecciones dominadas (con evaluación aprobada): {stats.Mastered}");
            sb.AppendLine($"- Evaluaciones presentadas: {stats.EvaluationsTaken}" +
                         (stats.AverageScore is double avg ? $" (promedio {avg:0.#} %)" : string.Empty));
            sb.AppendLine($"- Tiempo de estudio efectivo: {FormatMinutes(stats.TotalStudySeconds)} " +
                         $"(hoy {FormatMinutes(stats.StudySecondsToday)})");
            sb.AppendLine($"- Tickets: {stats.TicketsCreated} creados, {stats.TicketsResolved} resueltos/cerrados");
            sb.AppendLine();
            sb.AppendLine("## Habilidades demostradas (evaluaciones aprobadas)");
            if (stats.DemonstratedTopics.Count == 0)
                sb.AppendLine("- Aún no demostradas. Aprueba evaluaciones para evidenciarlas.");
            else
                foreach (var t in stats.DemonstratedTopics) sb.AppendLine($"- {t}");
            sb.AppendLine();
            sb.AppendLine("## Áreas de mejora (últimos resultados no aprobados o sin dominar)");
            if (stats.WeakTopics.Count == 0)
                sb.AppendLine("- Sin áreas detectadas aún.");
            else
                foreach (var t in stats.WeakTopics) sb.AppendLine($"- {t}");
            sb.AppendLine();
            sb.AppendLine("## Recomendaciones");
            sb.AppendLine("- Repasa los temas débiles antes de continuar con el siguiente bloque.");
            sb.AppendLine("- Repite las evaluaciones: el historial permite ver tu evolución.");
            sb.AppendLine("- Practica en el laboratorio y el simulador de tickets para aplicar lo aprendido.");
            sb.AppendLine();
            sb.AppendLine("## Próximos pasos");
            var pending = _catalog.Lessons.FirstOrDefault(l =>
                _learning.GetProgress(l.Id)?.State is null or Domain.LessonState.NotStarted or Domain.LessonState.InProgress);
            sb.AppendLine(pending is not null
                ? $"- Continuar con el día {pending.Day}: {pending.Title}"
                : "- Revisar el bloque de preparación laboral y comenzar la búsqueda de vacantes.");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine("Aviso: completar este programa no equivale a una certificación profesional " +
                          "ni garantiza empleo. Es una preparación de base para posiciones de entrada en soporte TI.");

            ReportText = sb.ToString();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Informe", "No se pudo generar el informe: " + ex.Message);
        }
    }

    private async Task ExportReportAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ReportText))
                GenerateReport();
            if (string.IsNullOrWhiteSpace(ReportText))
                return;

            var path = _dialogs.SaveFile("Documento Markdown (*.md)|*.md", "informe-ruta-ti-30.md");
            if (path is null) return;
            await File.WriteAllTextAsync(path, ReportText, Encoding.UTF8);
            _dialogs.Info("Informe", "Informe guardado en:\n" + path);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Informe", "No se pudo guardar el informe: " + ex.Message);
        }
    }

    private static string FormatMinutes(int seconds)
    {
        if (seconds <= 0) return "0 min";
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.TotalHours >= 1 ? $"{(int)ts.TotalHours} h {ts.Minutes:00} min" : $"{ts.Minutes} min";
    }
}
