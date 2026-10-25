using System.Collections.ObjectModel;
using RutaTI30.Application.Content;
using RutaTI30.App.Mvvm;

namespace RutaTI30.App.ViewModels;

/// <summary>Tarjeta de ruta en la lista.</summary>
public sealed record RouteItemVm(
    string Title,
    string Focus,
    string Prerequisites,
    string CompetenciesText,
    string LessonsText,
    string PracticesText,
    string AreasText,
    string EntryRoles);

/// <summary>Itinerarios opcionales posteriores al día 30. Todo el contenido es referencial y
/// se enlaza con lecciones, escenarios y áreas de entrevistas ya existentes.</summary>
public sealed class RoutesViewModel : ObservableObject, INavigableModule
{
    private readonly ILessonCatalog _lessons;

    public RoutesViewModel(ILessonCatalog lessons)
    {
        _lessons = lessons;
        foreach (var route in SpecializationRoutes.All)
            Items.Add(ToVm(route));
    }

    public ObservableCollection<RouteItemVm> Items { get; } = new();

    public Task RefreshAsync() => Task.CompletedTask;

    private RouteItemVm ToVm(SpecializationRoute route)
    {
        var lessons = route.LessonDays
            .Select(d => (Day: d, Title: _lessons.GetByDay(d)?.Title))
            .Where(x => !string.IsNullOrWhiteSpace(x.Title))
            .Select(x => $"· Día {x.Day}: {x.Title}")
            .ToList();
        var practices = TicketPracticeScenarios.All
            .Where(s => route.PracticeIds.Contains(s.Id))
            .Select(s => "· " + s.Title)
            .ToList();

        return new RouteItemVm(
            route.Title,
            route.Focus,
            route.Prerequisites,
            string.Join("\n", route.Competencies.Select(c => "· " + c)),
            string.Join("\n", lessons),
            string.Join("\n", practices),
            string.Join(" · ", route.InterviewAreas),
            route.EntryRoles);
    }
}
