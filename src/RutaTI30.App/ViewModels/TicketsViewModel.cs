using System.Collections.ObjectModel;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;
using RutaTI30.Domain;

namespace RutaTI30.App.ViewModels;

/// <summary>Opción de etiqueta para enums (etiquetas en español).</summary>
public sealed record OptionLabel<T>(T Value, string Label) where T : struct, Enum;

/// <summary>Ticket en la lista lateral.</summary>
public sealed record TicketItemVm(string Id, string Reporter, string Preview, string StateLabel,
    string PriorityLabel, string CreatedLocal);

/// <summary>Entrada del historial visible.</summary>
public sealed record HistoryVm(string WhenLocal, string Action, string Note, string Transition);

/// <summary>Simulador de Help Desk: CRUD con estados validados, historial y prioridad calculada.</summary>
public sealed class TicketsViewModel : ObservableObject, INavigableModule
{
    private readonly ITicketStore _store;
    private readonly IClock _clock;
    private readonly DialogService _dialogs;

    public ObservableCollection<TicketItemVm> Tickets { get; } = new();
    public ObservableCollection<HistoryVm> History { get; } = new();

    /// <summary>Casos de práctica cargables en el formulario (datos ficticios educativos).</summary>
    public IReadOnlyList<TicketPracticeScenario> PracticeCases { get; } = TicketPracticeScenarios.All;

    private TicketPracticeScenario? _selectedPracticeCase;
    public TicketPracticeScenario? SelectedPracticeCase
    {
        get => _selectedPracticeCase;
        set => SetProperty(ref _selectedPracticeCase, value);
    }

    public ObservableCollection<string> PracticeGuide { get; } = new();

    public IReadOnlyList<OptionLabel<TicketState>> StateFilters { get; } = new List<OptionLabel<TicketState>>
    {
        new((TicketState)(-1), "Todos los estados")
    }.Concat(Enum.GetValues<TicketState>().Select(s => new OptionLabel<TicketState>(s, StateName(s))))
        .ToList();

    private OptionLabel<TicketState> _selectedStateFilter;
    public OptionLabel<TicketState> SelectedStateFilter
    {
        get => _selectedStateFilter;
        set { if (SetProperty(ref _selectedStateFilter, value)) _ = RefreshAsync(); }
    }

    private string _searchText = string.Empty;
    public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }

    public IReadOnlyList<OptionLabel<TicketCategory>> CategoryOptions { get; } =
        Enum.GetValues<TicketCategory>().Select(c => new OptionLabel<TicketCategory>(c, CategoryName(c))).ToList();

    public IReadOnlyList<OptionLabel<ImpactLevel>> ImpactOptions { get; } =
        Enum.GetValues<ImpactLevel>().Select(i => new OptionLabel<ImpactLevel>(i, i.ToString())).ToList();

    public IReadOnlyList<OptionLabel<UrgencyLevel>> UrgencyOptions { get; } =
        Enum.GetValues<UrgencyLevel>().Select(u => new OptionLabel<UrgencyLevel>(u, u.ToString())).ToList();

    // --- Editor ---
    private bool _isNew = true;
    private string _editId = string.Empty;
    private string _reporter = string.Empty;
    private string _department = string.Empty;
    private string _description = string.Empty;
    private OptionLabel<TicketCategory> _category;
    private OptionLabel<ImpactLevel> _impact;
    private OptionLabel<UrgencyLevel> _urgency;
    private string _stateText = string.Empty;
    private string _priorityText = string.Empty;
    private string _resolution = string.Empty;
    private string _noteText = string.Empty;
    private string _statusMessage = string.Empty;
    private int _targetHours = 8;

    public bool IsNew { get => _isNew; private set => SetProperty(ref _isNew, value); }
    public string EditId { get => _editId; private set => SetProperty(ref _editId, value); }
    public string Reporter { get => _reporter; set => SetProperty(ref _reporter, value); }
    public string Department { get => _department; set => SetProperty(ref _department, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }

    public OptionLabel<TicketCategory> Category
    {
        get => _category;
        set { if (SetProperty(ref _category, value)) OnPropertyChanged(nameof(PriorityText)); }
    }

    public OptionLabel<ImpactLevel> Impact
    {
        get => _impact;
        set { if (SetProperty(ref _impact, value)) UpdatePriorityText(); }
    }

    public OptionLabel<UrgencyLevel> Urgency
    {
        get => _urgency;
        set { if (SetProperty(ref _urgency, value)) UpdatePriorityText(); }
    }

    public string StateText { get => _stateText; private set => SetProperty(ref _stateText, value); }
    public string PriorityText { get => _priorityText; private set => SetProperty(ref _priorityText, value); }
    public string Resolution { get => _resolution; set => SetProperty(ref _resolution, value); }
    public string NoteText { get => _noteText; set => SetProperty(ref _noteText, value); }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public int TargetHours { get => _targetHours; set => SetProperty(ref _targetHours, value); }

    public IReadOnlyList<OptionLabel<TicketState>> NextStateOptions { get; private set; } =
        Array.Empty<OptionLabel<TicketState>>();

    private OptionLabel<TicketState>? _selectedNextState;
    public OptionLabel<TicketState>? SelectedNextState
    {
        get => _selectedNextState;
        set => SetProperty(ref _selectedNextState, value);
    }

    private TicketItemVm? _selectedTicket;
    public TicketItemVm? SelectedTicket
    {
        get => _selectedTicket;
        set
        {
            if (SetProperty(ref _selectedTicket, value) && value is not null)
                LoadTicket(value.Id);
        }
    }

    public AsyncRelayCommand NewTicketCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand ChangeStateCommand { get; }
    public AsyncRelayCommand AddNoteCommand { get; }
    public AsyncRelayCommand EscalateCommand { get; }
    public AsyncRelayCommand ResolveCommand { get; }
    public AsyncRelayCommand SearchCommand { get; }
    public AsyncRelayCommand LoadPracticeCaseCommand { get; }

    public TicketsViewModel(ITicketStore store, IClock clock, DialogService dialogs)
    {
        _store = store;
        _clock = clock;
        _dialogs = dialogs;
        _selectedStateFilter = StateFilters[0];
        _category = CategoryOptions[0];
        _impact = ImpactOptions[1];
        _urgency = UrgencyOptions[1];

        NewTicketCommand = new AsyncRelayCommand(NewTicketAsync);
        LoadPracticeCaseCommand = new AsyncRelayCommand(LoadPracticeCaseAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ChangeStateCommand = new AsyncRelayCommand(ChangeStateAsync);
        AddNoteCommand = new AsyncRelayCommand(AddNoteAsync);
        EscalateCommand = new AsyncRelayCommand(EscalateAsync);
        ResolveCommand = new AsyncRelayCommand(ResolveAsync);
        SearchCommand = new AsyncRelayCommand(() => RefreshAsync());
        UpdatePriorityText();
    }

    public Task RefreshAsync()
    {
        try
        {
            var filter = SelectedStateFilter?.Value ?? default;
            TicketState? state = SelectedStateFilter is not null && (int)filter >= 0 ? filter : null;
            var items = _store.List(state, string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);

            Tickets.Clear();
            foreach (var t in items)
            {
                Tickets.Add(new TicketItemVm(
                    t.Id,
                    t.Reporter,
                    t.Description.Length > 70 ? t.Description[..70] + "…" : t.Description,
                    StateName(t.State),
                    PriorityName(t.Priority),
                    t.CreatedUtc.ToLocalTime().ToString("dd/MM HH:mm")));
            }

            if (_editId.Length > 0 && Tickets.All(t => t.Id != _editId) && !IsNew)
            {
                // El ticket editado quedó fuera del filtro: conservar editor pero avisar.
                StatusMessage = "El ticket actual no está en el filtro visible.";
            }
        }
        catch (Exception ex)
        {
            _dialogs.Error("Tickets", "No se pudo cargar la lista: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private Task NewTicketAsync()
    {
        IsNew = true;
        EditId = "(se asigna al guardar)";
        Reporter = string.Empty;
        Department = string.Empty;
        Description = string.Empty;
        Category = CategoryOptions[0];
        Impact = ImpactOptions[1];
        Urgency = UrgencyOptions[1];
        Resolution = string.Empty;
        NoteText = string.Empty;
        StateText = StateName(TicketState.Nuevo);
        TargetHours = 8;
        History.Clear();
        SelectedNextState = null;
        NextStateOptions = Array.Empty<OptionLabel<TicketState>>();
        OnPropertyChanged(nameof(NextStateOptions));
        PracticeGuide.Clear();
        SelectedPracticeCase = null;
        StatusMessage = "Nuevo ticket: completa los datos y guarda.";
        return Task.CompletedTask;
    }

    /// <summary>Carga un caso de práctica en el formulario: el estudiante investiga y guarda el ticket;
    /// la guía se muestra aparte para consultarla cuando él lo decida.</summary>
    private Task LoadPracticeCaseAsync()
    {
        if (SelectedPracticeCase is null)
        {
            StatusMessage = "Selecciona un caso de práctica y luego pulsa «Cargar caso».";
            return Task.CompletedTask;
        }

        var c = SelectedPracticeCase;
        IsNew = true;
        EditId = "(se asigna al guardar)";
        Reporter = c.Reporter;
        Department = c.Department;
        Description = c.Description;
        Category = CategoryOptions.First(o => o.Value == c.Category);
        Impact = ImpactOptions.First(o => o.Value == c.Impact);
        Urgency = UrgencyOptions.First(o => o.Value == c.Urgency);
        Resolution = string.Empty;
        NoteText = string.Empty;
        StateText = StateName(TicketState.Nuevo);
        TargetHours = 8;
        History.Clear();
        SelectedNextState = null;
        NextStateOptions = Array.Empty<OptionLabel<TicketState>>();
        OnPropertyChanged(nameof(NextStateOptions));

        PracticeGuide.Clear();
        PracticeGuide.Add("Preguntas de diagnóstico (investiga antes de mirar los pasos):");
        foreach (var q in c.DiagnosticQuestions) PracticeGuide.Add("  • " + q);
        PracticeGuide.Add("Pasos sugeridos:");
        foreach (var s in c.SuggestedSteps) PracticeGuide.Add("  • " + s);
        PracticeGuide.Add("Solución o escalamiento: " + c.ResolutionOrEscalation);
        PracticeGuide.Add("Documentación del ticket: " + c.DocumentationNote);
        StatusMessage = $"Caso «{c.Title}» cargado. Investiga, documenta y guarda; la guía queda abajo.";
        return Task.CompletedTask;
    }

    private async Task SaveAsync()
    {
        try
        {
            var errors = TicketRules.Validate(Reporter, Description);
            if (errors.Count > 0)
            {
                StatusMessage = string.Join(" ", errors);
                _dialogs.Error("Validación", StatusMessage);
                return;
            }

            var now = _clock.UtcNow;
            if (IsNew)
            {
                var ticket = new Ticket
                {
                    Id = NextId(),
                    CreatedUtc = now,
                    UpdatedUtc = now,
                    Reporter = Reporter.Trim(),
                    Department = Department.Trim(),
                    Description = Description.Trim(),
                    Category = Category.Value,
                    Impact = Impact.Value,
                    Urgency = Urgency.Value,
                    Priority = TicketRules.CalculatePriority(Impact.Value, Urgency.Value),
                    State = TicketState.Nuevo,
                    TargetResponseHours = TargetHours
                };
                _store.AddWithHistory(ticket, new TicketHistoryEntry
                {
                    TicketId = ticket.Id,
                    AtUtc = now,
                    Action = "Alta del ticket",
                    Note = "Ticket creado en el simulador (datos ficticios).",
                    ToState = TicketState.Nuevo
                });
                IsNew = false;
                EditId = ticket.Id;
                StatusMessage = $"Ticket {ticket.Id} creado.";
            }
            else
            {
                var ticket = _store.Get(EditId);
                if (ticket is null)
                {
                    StatusMessage = "El ticket ya no existe.";
                    return;
                }
                ticket.UpdatedUtc = now;
                ticket.Reporter = Reporter.Trim();
                ticket.Department = Department.Trim();
                ticket.Description = Description.Trim();
                ticket.Category = Category.Value;
                ticket.Impact = Impact.Value;
                ticket.Urgency = Urgency.Value;
                ticket.Priority = TicketRules.CalculatePriority(Impact.Value, Urgency.Value);
                ticket.TargetResponseHours = TargetHours;
                _store.Update(ticket);
                _store.AddHistory(new TicketHistoryEntry
                {
                    TicketId = ticket.Id,
                    AtUtc = now,
                    Action = "Actualización de datos",
                    Note = "Cambio de campos del ticket.",
                    FromState = ticket.State,
                    ToState = ticket.State
                });
                StatusMessage = $"Ticket {ticket.Id} actualizado.";
            }

            await RefreshAsync();
            LoadTicket(EditId);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Tickets", "No se pudo guardar: " + ex.Message);
        }
    }

    private async Task ChangeStateAsync()
    {
        try
        {
            var ticket = IsNew ? null : _store.Get(EditId);
            if (ticket is null)
            {
                StatusMessage = "Guarda el ticket antes de cambiar su estado.";
                return;
            }
            if (SelectedNextState is null)
            {
                StatusMessage = "Selecciona el estado destino.";
                return;
            }

            var target = SelectedNextState.Value;
            if (!TicketRules.CanTransition(ticket.State, target))
            {
                StatusMessage = $"Transición no permitida: {StateName(ticket.State)} → {StateName(target)}.";
                _dialogs.Error("Transición inválida",
                    $"Desde «{StateName(ticket.State)}» no se puede pasar a «{StateName(target)}».");
                return;
            }

            if (target == TicketState.Resuelto && string.IsNullOrWhiteSpace(Resolution))
            {
                StatusMessage = "Escribe la resolución antes de marcar el ticket como resuelto.";
                _dialogs.Error("Resolución requerida",
                    "Un ticket solo puede resolverse con una explicación de la solución.");
                return;
            }

            var previous = ticket.State;
            ticket.State = target;
            ticket.UpdatedUtc = _clock.UtcNow;
            if (target == TicketState.Escalado) ticket.Escalated = true;
            if (target == TicketState.Resuelto) ticket.Resolution = Resolution.Trim();
            _store.Update(ticket);
            _store.AddHistory(new TicketHistoryEntry
            {
                TicketId = ticket.Id,
                AtUtc = _clock.UtcNow,
                Action = "Cambio de estado",
                Note = NoteText.Trim(),
                FromState = previous,
                ToState = target
            });

            StatusMessage = $"Estado actualizado a {StateName(target)}.";
            NoteText = string.Empty;
            await RefreshAsync();
            LoadTicket(ticket.Id);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Tickets", "No se pudo cambiar el estado: " + ex.Message);
        }
    }

    private async Task AddNoteAsync()
    {
        try
        {
            if (IsNew || _store.Get(EditId) is null)
            {
                StatusMessage = "Guarda el ticket antes de añadir notas.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NoteText))
            {
                StatusMessage = "La nota no puede estar vacía.";
                return;
            }
            _store.AddHistory(new TicketHistoryEntry
            {
                TicketId = EditId,
                AtUtc = _clock.UtcNow,
                Action = "Nota técnica",
                Note = NoteText.Trim()
            });
            NoteText = string.Empty;
            StatusMessage = "Nota añadida al historial.";
            LoadTicket(EditId);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Tickets", "No se pudo añadir la nota: " + ex.Message);
        }
    }

    private async Task EscalateAsync()
    {
        SelectedNextState = NextStateOptions.FirstOrDefault(o => o.Value == TicketState.Escalado);
        if (SelectedNextState is null)
            StatusMessage = "El escalamiento no está disponible desde el estado actual.";
        else
            await ChangeStateAsync();
    }

    private async Task ResolveAsync()
    {
        SelectedNextState = NextStateOptions.FirstOrDefault(o => o.Value == TicketState.Resuelto);
        if (SelectedNextState is null)
            StatusMessage = "La resolución no está disponible desde el estado actual.";
        else
            await ChangeStateAsync();
    }

    private void LoadTicket(string id)
    {
        try
        {
            var ticket = _store.Get(id);
            if (ticket is null)
            {
                StatusMessage = "El ticket solicitado no existe.";
                return;
            }

            IsNew = false;
            EditId = ticket.Id;
            Reporter = ticket.Reporter;
            Department = ticket.Department;
            Description = ticket.Description;
            Category = CategoryOptions.First(c => c.Value == ticket.Category);
            Impact = ImpactOptions.First(i => i.Value == ticket.Impact);
            Urgency = UrgencyOptions.First(u => u.Value == ticket.Urgency);
            StateText = StateName(ticket.State);
            PriorityText = PriorityName(ticket.Priority) +
                           $" (impacto {ticket.Impact} × urgencia {ticket.Urgency})";
            Resolution = ticket.Resolution;
            TargetHours = ticket.TargetResponseHours;

            NextStateOptions = Enum.GetValues<TicketState>()
                .Where(s => TicketRules.CanTransition(ticket.State, s))
                .Select(s => new OptionLabel<TicketState>(s, StateName(s)))
                .ToList();
            OnPropertyChanged(nameof(NextStateOptions));
            SelectedNextState = NextStateOptions.FirstOrDefault();

            History.Clear();
            foreach (var entry in _store.GetHistory(ticket.Id))
            {
                History.Add(new HistoryVm(
                    entry.AtUtc.ToLocalTime().ToString("dd/MM HH:mm"),
                    entry.Action,
                    entry.Note,
                    entry.FromState is null && entry.ToState is null
                        ? string.Empty
                        : $"{(entry.FromState is null ? "—" : StateName(entry.FromState.Value))} → " +
                          $"{(entry.ToState is null ? "—" : StateName(entry.ToState.Value))}"));
            }
        }
        catch (Exception ex)
        {
            _dialogs.Error("Tickets", "No se pudo abrir el ticket: " + ex.Message);
        }
    }

    private void UpdatePriorityText()
    {
        var priority = TicketRules.CalculatePriority(Impact?.Value ?? default, Urgency?.Value ?? default);
        PriorityText = $"{PriorityName(priority)} · {Impact?.Value ?? default} × {Urgency?.Value ?? default}";
    }

    private string NextId()
    {
        var stamp = _clock.UtcNow.ToString("yyyyMMdd");
        int sequential = _store.List().Count(t => t.Id.StartsWith($"INC-{stamp}")) + 1;
        return $"INC-{stamp}-{sequential:000}";
    }

    public static string StateName(TicketState state) => state switch
    {
        TicketState.Nuevo => "Nuevo",
        TicketState.EnAtencion => "En atención",
        TicketState.EnEsperaUsuario => "En espera de usuario",
        TicketState.Escalado => "Escalado",
        TicketState.Resuelto => "Resuelto",
        TicketState.Cerrado => "Cerrado",
        _ => state.ToString()
    };

    public static string PriorityName(PriorityLevel priority) => priority switch
    {
        PriorityLevel.P1 => "P1 · Crítica",
        PriorityLevel.P2 => "P2 · Alta",
        PriorityLevel.P3 => "P3 · Media",
        PriorityLevel.P4 => "P4 · Baja",
        _ => priority.ToString()
    };

    public static string CategoryName(TicketCategory category) => category switch
    {
        TicketCategory.Redes => "Redes",
        TicketCategory.Acceso => "Acceso / identidades",
        TicketCategory.Aplicacion => "Aplicación",
        TicketCategory.Impresora => "Impresora",
        TicketCategory.Hardware => "Hardware",
        TicketCategory.Servicio => "Servicio / sistema",
        _ => "Otro"
    };
}
