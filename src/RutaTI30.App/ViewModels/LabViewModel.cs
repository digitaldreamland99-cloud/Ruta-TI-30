using System.Collections.ObjectModel;
using System.Text.Json;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;
using RutaTI30.Domain;

namespace RutaTI30.App.ViewModels;

/// <summary>Escenario en la lista del laboratorio.</summary>
public sealed record ScenarioItemVm(string Id, string Title, string DifficultyLabel);

/// <summary>Evidencia simulada clave/valor.</summary>
public sealed record EvidenceVm(string Key, string Value);

/// <summary>Acción disponible en el escenario.</summary>
public sealed record ActionVm(string Id, string Label, string KindLabel, bool RequiresElevation);

/// <summary>Decisión tomada con su retroalimentación.</summary>
public sealed class DecisionVm
{
    public string Label { get; init; } = string.Empty;
    public string Feedback { get; init; } = string.Empty;
    public string WeightText { get; init; } = string.Empty;
}

/// <summary>Laboratorio de soporte: escenarios simulados donde las decisiones cambian el resultado.</summary>
public sealed class LabViewModel : ObservableObject, INavigableModule
{
    private readonly ILabCatalog _catalog;
    private readonly ILabStore _store;
    private readonly DialogService _dialogs;
    private readonly IClock _clock;

    public ObservableCollection<ScenarioItemVm> Scenarios { get; } = new();

    private ScenarioItemVm? _selectedScenario;
    public ScenarioItemVm? SelectedScenario
    {
        get => _selectedScenario;
        set
        {
            if (SetProperty(ref _selectedScenario, value) && value is not null)
                LoadScenario(value.Id);
        }
    }

    private bool _hasScenario;
    private string _title = string.Empty;
    private string _context = string.Empty;
    private string _difficulty = string.Empty;
    private string _resultText = string.Empty;
    private string _solutionText = string.Empty;
    private string _escalationText = string.Empty;
    private bool _finished;

    public bool HasScenario { get => _hasScenario; private set => SetProperty(ref _hasScenario, value); }
    public string Title { get => _title; private set => SetProperty(ref _title, value); }
    public string Context { get => _context; private set => SetProperty(ref _context, value); }
    public string Difficulty { get => _difficulty; private set => SetProperty(ref _difficulty, value); }
    public string ResultText { get => _resultText; private set => SetProperty(ref _resultText, value); }
    public string SolutionText { get => _solutionText; private set => SetProperty(ref _solutionText, value); }
    public string EscalationText { get => _escalationText; private set => SetProperty(ref _escalationText, value); }
    public bool Finished { get => _finished; private set => SetProperty(ref _finished, value); }

    public ObservableCollection<string> Symptoms { get; } = new();
    public ObservableCollection<EvidenceVm> Evidence { get; } = new();
    public ObservableCollection<ActionVm> Actions { get; } = new();
    public ObservableCollection<DecisionVm> Decisions { get; } = new();
    public ObservableCollection<string> Criteria { get; } = new();

    public AsyncRelayCommand ChooseActionCommand { get; }
    public AsyncRelayCommand FinishCommand { get; }
    public AsyncRelayCommand ResetCommand { get; }

    private LabScenario? _current;
    private LabAttempt? _attempt;
    private readonly List<string> _chosen = new();

    public LabViewModel(ILabCatalog catalog, ILabStore store, DialogService dialogs, IClock clock)
    {
        _catalog = catalog;
        _store = store;
        _dialogs = dialogs;
        _clock = clock;
        ChooseActionCommand = new AsyncRelayCommand(ChooseActionAsync);
        FinishCommand = new AsyncRelayCommand(FinishAsync);
        ResetCommand = new AsyncRelayCommand(ResetAsync);
    }

    public Task RefreshAsync()
    {
        try
        {
            Scenarios.Clear();
            foreach (var s in _catalog.Scenarios)
            {
                Scenarios.Add(new ScenarioItemVm(s.Id, s.Title, $"Dificultad {s.Difficulty}/5"));
            }
            _selectedScenario ??= Scenarios.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedScenario));
            if (_selectedScenario is not null && _current?.Id != _selectedScenario.Id)
                LoadScenario(_selectedScenario.Id);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Laboratorio", "No se pudieron cargar los escenarios: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private void LoadScenario(string id)
    {
        var scenario = _catalog.GetById(id);
        if (scenario is null)
        {
            HasScenario = false;
            return;
        }

        _current = scenario;
        _chosen.Clear();
        HasScenario = true;
        Finished = false;
        ResultText = string.Empty;
        SolutionText = string.Empty;
        EscalationText = string.Empty;
        Title = scenario.Title;
        Context = scenario.Context;
        Difficulty = $"Dificultad {scenario.Difficulty}/5 · modo simulado (no toca tu equipo)";

        Symptoms.Clear();
        foreach (var s in scenario.Symptoms) Symptoms.Add(s);

        Evidence.Clear();
        foreach (var e in scenario.InitialData) Evidence.Add(new EvidenceVm(e.Key, e.Value));

        Actions.Clear();
        foreach (var a in scenario.Actions)
        {
            Actions.Add(new ActionVm(a.Id, a.Label,
                a.Kind == "Cambio" ? "Cambio (requiere autorización en la vida real)" : "Diagnóstico de solo lectura",
                a.RequiresElevation));
        }

        Decisions.Clear();
        Criteria.Clear();
        foreach (var c in scenario.EvaluationCriteria) Criteria.Add(c);

        _attempt = new LabAttempt
        {
            ScenarioId = scenario.Id,
            StartedUtc = _clock.UtcNow
        };
    }

    private Task ChooseActionAsync(object? parameter)
    {
        if (_current is null || Finished) return Task.CompletedTask;
        if (parameter is not ActionVm action)
            return Task.CompletedTask;
        if (_chosen.Contains(action.Id))
        {
            _dialogs.Info("Laboratorio", "Ya realizaste esta acción en este caso.");
            return Task.CompletedTask;
        }

        var definition = _current.Actions.FirstOrDefault(a => a.Id == action.Id);
        if (definition is null) return Task.CompletedTask;

        _chosen.Add(action.Id);
        foreach (var e in definition.Evidence)
        {
            var existing = Evidence.FirstOrDefault(x => x.Key == e.Key);
            if (existing is not null)
            {
                Evidence.Remove(existing);
            }
            Evidence.Add(new EvidenceVm(e.Key, e.Value));
        }

        Decisions.Add(new DecisionVm
        {
            Label = action.Label,
            Feedback = definition.Feedback,
            WeightText = definition.ScoreWeight > 0
                ? "decisión recomendada"
                : definition.ScoreWeight < 0
                    ? "decisión insegura o prematura"
                    : "acción neutra"
        });
        return Task.CompletedTask;
    }

    private async Task FinishAsync()
    {
        if (_current is null || _attempt is null) return;
        if (_chosen.Count == 0)
        {
            _dialogs.Info("Laboratorio", "Toma al menos una decisión antes de finalizar el caso.");
            return;
        }

        try
        {
            var score = LabScoring.Score(_current, _chosen);
            _attempt.FinishedUtc = _clock.UtcNow;
            _attempt.Score = score.Percentage;
            _attempt.DecisionsJson = JsonSerializer.Serialize(_chosen);
            if (_attempt.Id == 0) _store.AddAttempt(_attempt);
            else _store.UpdateAttempt(_attempt);

            ResultText =
                $"Puntaje: {score.Percentage:0.#} % ({score.Points:0}/{score.MaxPoints:0}) · " +
                (score.Passed ? "caso resuelto adecuadamente" : "caso con margen de mejora") +
                (score.UnsafeActions.Count > 0
                    ? $" · Acciones inseguras: {string.Join("; ", score.UnsafeActions)}"
                    : string.Empty) +
                (score.RecommendedMissed.Count > 0
                    ? $" · Pasos recomendados omitidos: {string.Join("; ", score.RecommendedMissed)}"
                    : string.Empty);
            SolutionText = "Solución explicada: " + _current.Solution;
            EscalationText = "Cuándo escalar: " + _current.EscalationGuidance;
            Finished = true;
        }
        catch (Exception ex)
        {
            _dialogs.Error("Laboratorio", "No se pudo finalizar el caso: " + ex.Message);
        }
        await Task.CompletedTask;
    }

    private Task ResetAsync()
    {
        if (_current is null) return Task.CompletedTask;
        if (!_dialogs.Confirm("Reiniciar caso",
                "Se perderán las decisiones de este intento (los intentos guardados permanecen). ¿Continuar?"))
            return Task.CompletedTask;
        LoadScenario(_current.Id);
        return Task.CompletedTask;
    }
}
