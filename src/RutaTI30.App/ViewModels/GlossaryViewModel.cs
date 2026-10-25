using System.Collections.ObjectModel;
using System.Text;
using RutaTI30.Application.Content;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;

namespace RutaTI30.App.ViewModels;

/// <summary>Fila de resultado del glosario.</summary>
public sealed record GlossaryItemVm(
    string Term,
    string Category,
    string SimpleDefinition,
    string TechnicalExplanation,
    string WorkplaceExample,
    string CommonMistake,
    string RelatedText);

/// <summary>Biblioteca local de consulta: búsqueda sin Internet, filtro por categoría y
/// enlaces con las lecciones del programa.</summary>
public sealed class GlossaryViewModel : ObservableObject, INavigableModule
{
    private readonly IGlossaryCatalog _glossary;
    private readonly ILessonCatalog _lessons;
    private readonly DialogService _dialogs;

    private string _searchText = string.Empty;
    private string? _selectedCategory;
    private string _resultSummary = string.Empty;
    private GlossaryItemVm? _selectedItem;

    public GlossaryViewModel(IGlossaryCatalog glossary, ILessonCatalog lessons, DialogService dialogs)
    {
        _glossary = glossary;
        _lessons = lessons;
        _dialogs = dialogs;
        Categories.Add("Todas");
        foreach (var c in _glossary.Categories) Categories.Add(c);
        _selectedCategory = "Todas";
        ClearSearchCommand = new RelayCommand(() => { SearchText = string.Empty; SelectedCategory = "Todas"; });
    }

    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<GlossaryItemVm> Items { get; } = new();

    public RelayCommand ClearSearchCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) RefreshItems(); }
    }

    public string? SelectedCategory
    {
        get => _selectedCategory;
        set { if (SetProperty(ref _selectedCategory, value)) RefreshItems(); }
    }

    public string ResultSummary
    {
        get => _resultSummary;
        private set => SetProperty(ref _resultSummary, value);
    }

    public GlossaryItemVm? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public Task RefreshAsync()
    {
        RefreshItems();
        return Task.CompletedTask;
    }

    private void RefreshItems()
    {
        try
        {
            var category = SelectedCategory == "Todas" ? null : SelectedCategory;
            var results = _glossary.Search(SearchText, category);

            Items.Clear();
            foreach (var e in results)
                Items.Add(ToVm(e));

            ResultSummary = results.Count == 0
                ? "Sin resultados: prueba con otra palabra (por ejemplo «DNS», «ticket» o «contraseña»)."
                : $"{results.Count} concepto(s) — la búsqueda funciona sin conexión.";
        }
        catch (Exception ex)
        {
            _dialogs.Error("Glosario", "No se pudo buscar en el glosario: " + ex.Message);
        }
    }

    private GlossaryItemVm ToVm(GlossaryEntry e)
    {
        var related = new StringBuilder();
        var lessons = e.RelatedLessonDays
            .Select(d => _lessons.GetByDay(d))
            .Where(l => l is not null)
            .ToList();
        if (lessons.Count > 0)
            related.Append("Lecciones: ")
                .AppendJoin(", ", lessons.Select(l => $"día {l!.Day}"))
                .AppendLine();
        if (e.RelatedTermIds.Count > 0)
        {
            var terms = e.RelatedTermIds
                .Select(id => _glossary.GetById(id)?.Term)
                .Where(t => !string.IsNullOrWhiteSpace(t));
            related.Append("Conceptos relacionados: ").AppendJoin(", ", terms);
        }

        return new GlossaryItemVm(
            e.Term,
            e.Category,
            e.SimpleDefinition,
            e.TechnicalExplanation,
            e.WorkplaceExample,
            e.CommonMistake,
            related.ToString().Trim());
    }
}
