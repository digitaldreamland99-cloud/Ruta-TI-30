using System.Collections.ObjectModel;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;
using RutaTI30.Domain;

namespace RutaTI30.App.ViewModels;

/// <summary>Pregunta en la lista del banco de entrevistas.</summary>
public sealed record QuestionItemVm(string Id, string KindLabel, string Area, string Statement);

/// <summary>Intento guardado visible.</summary>
public sealed record AttemptVm(string WhenLocal, string Rating, string AnswerPreview);

/// <summary>Postulación en la lista.</summary>
public sealed record ApplicationItemVm(long Id, string Company, string Role, string StateLabel, string NextFollowUp);

/// <summary>Preparación laboral: banco de entrevistas y registro de postulaciones.</summary>
public sealed class CareerViewModel : ObservableObject, INavigableModule
{
    private readonly IInterviewBank _bank;
    private readonly IJobStore _store;
    private readonly IClock _clock;
    private readonly DialogService _dialogs;

    // --- Entrevistas ---
    public ObservableCollection<QuestionItemVm> Questions { get; } = new();
    public ObservableCollection<string> KeyConcepts { get; } = new();
    public ObservableCollection<string> CommonMistakes { get; } = new();
    public ObservableCollection<string> Criteria { get; } = new();
    public ObservableCollection<string> FollowUps { get; } = new();
    public ObservableCollection<AttemptVm> AnswerHistory { get; } = new();

    /// <summary>Guía calculada de fortalezas y temas a repasar a partir de las autoevaluaciones guardadas.</summary>
    public ObservableCollection<string> InterviewGuide { get; } = new();

    private QuestionItemVm? _selectedQuestion;
    public QuestionItemVm? SelectedQuestion
    {
        get => _selectedQuestion;
        set { if (SetProperty(ref _selectedQuestion, value) && value is not null) LoadQuestion(value.Id); }
    }

    private bool _hasQuestion;
    private string _statement = string.Empty;
    private string _modelAnswer = string.Empty;
    private string _explanation = string.Empty;
    private string _answerText = string.Empty;
    private string _notesText = string.Empty;
    private int? _selfRating;
    private string _statusMessage = string.Empty;

    public bool HasQuestion { get => _hasQuestion; private set => SetProperty(ref _hasQuestion, value); }
    public string Statement { get => _statement; private set => SetProperty(ref _statement, value); }
    public string ModelAnswer { get => _modelAnswer; private set => SetProperty(ref _modelAnswer, value); }
    public string Explanation { get => _explanation; private set => SetProperty(ref _explanation, value); }
    public string AnswerText { get => _answerText; set => SetProperty(ref _answerText, value); }
    public string NotesText { get => _notesText; set => SetProperty(ref _notesText, value); }
    public int? SelfRating { get => _selfRating; set => SetProperty(ref _selfRating, value); }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }

    public IReadOnlyList<int> RatingOptions { get; } = new List<int> { 1, 2, 3, 4, 5 };

    public AsyncRelayCommand SaveAnswerCommand { get; }

    // --- Postulaciones ---
    public ObservableCollection<ApplicationItemVm> Applications { get; } = new();

    public IReadOnlyList<OptionLabel<JobState>> StateOptions { get; } =
        Enum.GetValues<JobState>().Select(s => new OptionLabel<JobState>(s, JobStateName(s))).ToList();

    private OptionLabel<JobState> _selectedStateFilter;
    public OptionLabel<JobState> SelectedStateFilter
    {
        get => _selectedStateFilter;
        set { if (SetProperty(ref _selectedStateFilter, value)) _ = RefreshAsync(); }
    }

    private ApplicationItemVm? _selectedApplication;
    public ApplicationItemVm? SelectedApplication
    {
        get => _selectedApplication;
        set { if (SetProperty(ref _selectedApplication, value) && value is not null) LoadApplication(value.Id); }
    }

    private bool _isNewApplication = true;
    private long _applicationId;
    private string _company = string.Empty;
    private string _role = string.Empty;
    private string _location = string.Empty;
    private DateTime? _discovered;
    private DateTime? _applied;
    private string _url = string.Empty;
    private string _salary = string.Empty;
    private OptionLabel<JobState> _jobState;
    private string _contact = string.Empty;
    private DateTime? _interview;
    private DateTime? _nextFollowUp;
    private string _jobNotes = string.Empty;
    private string _result = string.Empty;
    private string _followUpNote = string.Empty;
    private string _jobStatusMessage = string.Empty;

    public bool IsNewApplication { get => _isNewApplication; private set => SetProperty(ref _isNewApplication, value); }
    public string Company { get => _company; set => SetProperty(ref _company, value); }
    public string Role { get => _role; set => SetProperty(ref _role, value); }
    public string Location { get => _location; set => SetProperty(ref _location, value); }
    public DateTime? Discovered { get => _discovered; set => SetProperty(ref _discovered, value); }
    public DateTime? Applied { get => _applied; set => SetProperty(ref _applied, value); }
    public string Url { get => _url; set => SetProperty(ref _url, value); }
    public string Salary { get => _salary; set => SetProperty(ref _salary, value); }
    public OptionLabel<JobState> JobStateSelected
    {
        get => _jobState;
        set => SetProperty(ref _jobState, value);
    }
    public string Contact { get => _contact; set => SetProperty(ref _contact, value); }
    public DateTime? Interview { get => _interview; set => SetProperty(ref _interview, value); }
    public DateTime? NextFollowUp { get => _nextFollowUp; set => SetProperty(ref _nextFollowUp, value); }
    public string JobNotes { get => _jobNotes; set => SetProperty(ref _jobNotes, value); }
    public string Result { get => _result; set => SetProperty(ref _result, value); }
    public string FollowUpNote { get => _followUpNote; set => SetProperty(ref _followUpNote, value); }
    public string JobStatusMessage { get => _jobStatusMessage; private set => SetProperty(ref _jobStatusMessage, value); }

    public ObservableCollection<string> FollowUpHistory { get; } = new();

    public AsyncRelayCommand NewApplicationCommand { get; }
    public AsyncRelayCommand SaveApplicationCommand { get; }
    public AsyncRelayCommand AddFollowUpCommand { get; }
    public AsyncRelayCommand DeleteApplicationCommand { get; }

    public CareerViewModel(IInterviewBank bank, IJobStore store, IClock clock, DialogService dialogs)
    {
        _bank = bank;
        _store = store;
        _clock = clock;
        _dialogs = dialogs;
        _selectedStateFilter = new OptionLabel<JobState>((JobState)(-1), "Todos los estados");
        _jobState = StateOptions[0];

        SaveAnswerCommand = new AsyncRelayCommand(SaveAnswerAsync);
        NewApplicationCommand = new AsyncRelayCommand(NewApplicationAsync);
        SaveApplicationCommand = new AsyncRelayCommand(SaveApplicationAsync);
        AddFollowUpCommand = new AsyncRelayCommand(AddFollowUpAsync);
        DeleteApplicationCommand = new AsyncRelayCommand(DeleteApplicationAsync);
    }

    public Task RefreshAsync()
    {
        try
        {
            Questions.Clear();
            foreach (var q in _bank.Questions)
            {
                Questions.Add(new QuestionItemVm(q.Id, KindName(q.Kind), q.Area, q.Statement));
            }
            _selectedQuestion ??= Questions.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedQuestion));
            if (_selectedQuestion is not null)
                LoadQuestion(_selectedQuestion.Id);
            RebuildInterviewGuide();

            RefreshApplications();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Preparación laboral", "No se pudo cargar: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private void LoadQuestion(string id)
    {
        var question = _bank.GetById(id);
        if (question is null)
        {
            HasQuestion = false;
            return;
        }
        HasQuestion = true;
        Statement = question.Statement;
        ModelAnswer = question.ModelAnswer;
        Explanation = question.Explanation;

        Fill(KeyConcepts, question.KeyConcepts);
        Fill(CommonMistakes, question.CommonMistakes);
        Fill(Criteria, question.EvaluationCriteria);
        Fill(FollowUps, question.FollowUps);

        AnswerHistory.Clear();
        foreach (var attempt in _store.GetInterviewAttempts(id).Take(10))
        {
            AnswerHistory.Add(new AttemptVm(
                attempt.AttemptedUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                attempt.SelfRating is int r ? $"autoevaluación {r}/5" : "sin autoevaluación",
                attempt.AnswerText.Length > 80 ? attempt.AnswerText[..80] + "…" : attempt.AnswerText));
        }
    }

    /// <summary>Calcula la guía de fortalezas y repaso desde los intentos reales guardados.
    /// Sin autoevaluaciones no inventa nada: lo indica explícitamente.</summary>
    private void RebuildInterviewGuide()
    {
        InterviewGuide.Clear();
        var attempts = _store.GetInterviewAttempts();
        if (attempts.Count == 0)
        {
            InterviewGuide.Add("Aún no hay respuestas guardadas: practica una pregunta y autoevalúala para ver tu guía de fortalezas y repaso.");
            return;
        }

        var questionAreas = _bank.Questions
            .GroupBy(q => q.Id)
            .ToDictionary(g => g.Key, g => g.First().Area);

        var rated = attempts
            .Where(a => a.SelfRating is not null && questionAreas.ContainsKey(a.QuestionId))
            .GroupBy(a => questionAreas[a.QuestionId])
            .Select(g => new
            {
                Area = g.Key,
                Avg = g.Average(a => a.SelfRating!.Value),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Avg)
            .ToList();

        foreach (var area in rated.Where(x => x.Avg >= 4))
            InterviewGuide.Add($"Fortaleza: {area.Area} (autoevaluación media {area.Avg:0.0}/5 en {area.Count} intentos).");
        foreach (var area in rated.Where(x => x.Avg < 3))
            InterviewGuide.Add($"Tema a repasar: {area.Area} (autoevaluación media {area.Avg:0.0}/5 en {area.Count} intentos). Vuelve a la respuesta orientativa y practica de nuevo.");
        foreach (var area in rated.Where(x => x.Avg >= 3 && x.Avg < 4))
            InterviewGuide.Add($"En consolidación: {area.Area} (autoevaluación media {area.Avg:0.0}/5).");

        var practicadas = rated.Select(x => x.Area).ToHashSet();
        var pendientes = _bank.Questions.Select(q => q.Area).Distinct().Where(a => !practicadas.Contains(a)).ToList();
        if (pendientes.Count > 0)
            InterviewGuide.Add("Sin práctica todavía: " + string.Join(", ", pendientes) + ".");

        InterviewGuide.Add("La autoevaluación es una guía subjetiva de estudio; no es una métrica de contratación.");
    }

    private async Task SaveAnswerAsync()
    {
        try
        {
            if (_selectedQuestion is null)
            {
                StatusMessage = "Selecciona una pregunta.";
                return;
            }
            if (string.IsNullOrWhiteSpace(AnswerText))
            {
                StatusMessage = "Escribe tu respuesta antes de guardarla.";
                _dialogs.Error("Validación", "La respuesta no puede estar vacía.");
                return;
            }

            _store.AddInterviewAttempt(new InterviewAttempt
            {
                QuestionId = _selectedQuestion.Id,
                AttemptedUtc = _clock.UtcNow,
                AnswerText = AnswerText.Trim(),
                SelfRating = SelfRating,
                Notes = NotesText.Trim()
            });
            StatusMessage = "Respuesta guardada. Puedes consultarla en el historial y repetirla después.";
            AnswerText = string.Empty;
            NotesText = string.Empty;
            SelfRating = null;
            LoadQuestion(_selectedQuestion.Id);
            RebuildInterviewGuide(); // la guía de fortalezas/repaso se recalcula al instante con la nueva autoevaluación
        }
        catch (Exception ex)
        {
            _dialogs.Error("Entrevistas", "No se pudo guardar: " + ex.Message);
        }
        await Task.CompletedTask;
    }

    private void RefreshApplications()
    {
        var filter = SelectedStateFilter?.Value ?? default;
        var items = _store.ListApplications()
            .Where(a => (int)filter < 0 || a.State == filter)
            .ToList();

        Applications.Clear();
        foreach (var a in items)
        {
            Applications.Add(new ApplicationItemVm(
                a.Id,
                a.Company,
                a.Role,
                JobStateName(a.State),
                a.NextFollowUpUtc is DateTime f ? f.ToLocalTime().ToString("dd/MM/yyyy") : "—"));
        }
    }

    private Task NewApplicationAsync()
    {
        IsNewApplication = true;
        _applicationId = 0;
        Company = string.Empty;
        Role = string.Empty;
        Location = string.Empty;
        Discovered = _clock.LocalNow.Date;
        Applied = null;
        Url = string.Empty;
        Salary = string.Empty;
        JobStateSelected = StateOptions[0];
        Contact = string.Empty;
        Interview = null;
        NextFollowUp = null;
        JobNotes = string.Empty;
        Result = string.Empty;
        FollowUpHistory.Clear();
        JobStatusMessage = "Nueva postulación: registra los datos de la vacante.";
        return Task.CompletedTask;
    }

    private async Task SaveApplicationAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Company) || string.IsNullOrWhiteSpace(Role))
            {
                JobStatusMessage = "La empresa y el puesto son obligatorios.";
                _dialogs.Error("Validación", JobStatusMessage);
                return;
            }

            if (IsNewApplication || _applicationId == 0)
            {
                var app = new JobApplication
                {
                    Company = Company.Trim(),
                    Role = Role.Trim(),
                    Location = Location.Trim(),
                    DiscoveredUtc = Discovered?.ToUniversalTime(),
                    AppliedUtc = Applied?.ToUniversalTime(),
                    Url = Url.Trim(),
                    Salary = Salary.Trim(),
                    State = JobStateSelected.Value,
                    Contact = Contact.Trim(),
                    InterviewUtc = Interview?.ToUniversalTime(),
                    NextFollowUpUtc = NextFollowUp?.ToUniversalTime(),
                    Notes = JobNotes.Trim(),
                    Result = Result.Trim()
                };
                var errores = JobRules.Validate(app);
                if (errores.Count > 0)
                {
                    JobStatusMessage = string.Join(" ", errores);
                    _dialogs.Error("Validación", JobStatusMessage);
                    return;
                }
                _store.AddApplication(app);
                _applicationId = app.Id;
                IsNewApplication = false;
                JobStatusMessage = "Postulación registrada localmente. Esta aplicación no envía nada a empleadores.";
            }
            else
            {
                var app = _store.GetApplication(_applicationId);
                if (app is null)
                {
                    JobStatusMessage = "El registro ya no existe.";
                    return;
                }
                app.Company = Company.Trim();
                app.Role = Role.Trim();
                app.Location = Location.Trim();
                app.DiscoveredUtc = Discovered?.ToUniversalTime();
                app.AppliedUtc = Applied?.ToUniversalTime();
                app.Url = Url.Trim();
                app.Salary = Salary.Trim();
                app.State = JobStateSelected.Value;
                app.Contact = Contact.Trim();
                app.InterviewUtc = Interview?.ToUniversalTime();
                app.NextFollowUpUtc = NextFollowUp?.ToUniversalTime();
                app.Notes = JobNotes.Trim();
                app.Result = Result.Trim();
                var erroresEdicion = JobRules.Validate(app);
                if (erroresEdicion.Count > 0)
                {
                    JobStatusMessage = string.Join(" ", erroresEdicion);
                    _dialogs.Error("Validación", JobStatusMessage);
                    return;
                }
                _store.UpdateApplication(app);
                JobStatusMessage = "Postulación actualizada.";
            }

            RefreshApplications();
            LoadApplication(_applicationId);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Postulaciones", "No se pudo guardar: " + ex.Message);
        }
        await Task.CompletedTask;
    }

    /// <summary>Elimina la postulación seleccionada con confirmación explícita (y sus seguimientos).</summary>
    private async Task DeleteApplicationAsync()
    {
        try
        {
            if (IsNewApplication || _applicationId == 0)
            {
                JobStatusMessage = "Selecciona una postulación guardada para eliminarla.";
                return;
            }

            var app = _store.GetApplication(_applicationId);
            if (app is null)
            {
                JobStatusMessage = "El registro ya no existe.";
                return;
            }

            var confirmado = _dialogs.Confirm("Eliminar postulación",
                $"¿Eliminar la postulación a «{app.Role}» en «{app.Company}»?\n\n" +
                "Se eliminan también sus seguimientos. Esta acción no se puede deshacer.");
            if (!confirmado)
            {
                JobStatusMessage = "Eliminación cancelada; el registro queda intacto.";
                return;
            }

            _store.DeleteApplication(_applicationId);
            await NewApplicationAsync();
            RefreshApplications();
            JobStatusMessage = "Postulación eliminada junto con sus seguimientos.";
        }
        catch (Exception ex)
        {
            _dialogs.Error("Postulaciones", "No se pudo eliminar: " + ex.Message);
        }
    }

    private async Task AddFollowUpAsync()
    {
        try
        {
            if (_applicationId == 0)
            {
                JobStatusMessage = "Guarda la postulación antes de añadir seguimientos.";
                return;
            }
            if (string.IsNullOrWhiteSpace(FollowUpNote))
            {
                JobStatusMessage = "El seguimiento no puede estar vacío.";
                return;
            }
            _store.AddFollowUp(new FollowUp
            {
                ApplicationId = _applicationId,
                AtUtc = _clock.UtcNow,
                Note = FollowUpNote.Trim()
            });
            FollowUpNote = string.Empty;
            JobStatusMessage = "Seguimiento registrado.";
            LoadApplication(_applicationId);
            RefreshApplications();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Postulaciones", "No se pudo registrar el seguimiento: " + ex.Message);
        }
        await Task.CompletedTask;
    }

    private void LoadApplication(long id)
    {
        try
        {
            var app = _store.GetApplication(id);
            if (app is null) return;
            _applicationId = app.Id;
            IsNewApplication = false;
            Company = app.Company;
            Role = app.Role;
            Location = app.Location;
            Discovered = app.DiscoveredUtc?.ToLocalTime().Date;
            Applied = app.AppliedUtc?.ToLocalTime().Date;
            Url = app.Url;
            Salary = app.Salary;
            JobStateSelected = StateOptions.First(s => s.Value == app.State);
            Contact = app.Contact;
            Interview = app.InterviewUtc?.ToLocalTime();
            NextFollowUp = app.NextFollowUpUtc?.ToLocalTime().Date;
            JobNotes = app.Notes;
            Result = app.Result;

            FollowUpHistory.Clear();
            foreach (var f in _store.GetFollowUps(app.Id))
            {
                FollowUpHistory.Add($"{f.AtUtc.ToLocalTime():dd/MM/yyyy HH:mm} · {f.Note}");
            }
        }
        catch (Exception ex)
        {
            _dialogs.Error("Postulaciones", "No se pudo abrir el registro: " + ex.Message);
        }
    }

    private static void Fill(ObservableCollection<string> target, IReadOnlyList<string> source)
    {
        target.Clear();
        foreach (var item in source) target.Add(item);
    }

    public static string KindName(InterviewKind kind) => kind switch
    {
        InterviewKind.Tecnica => "Técnica",
        InterviewKind.Conductual => "Conductual",
        InterviewKind.Diagnostico => "Diagnóstico",
        _ => kind.ToString()
    };

    public static string JobStateName(JobState state) => state switch
    {
        JobState.PorRevisar => "Por revisar",
        JobState.PorPostular => "Por postular",
        JobState.Postulado => "Postulado",
        JobState.Entrevista => "Entrevista",
        JobState.EvaluacionTecnica => "Evaluación técnica",
        JobState.Oferta => "Oferta",
        JobState.NoSeleccionado => "No seleccionado",
        JobState.Cerrado => "Cerrado",
        _ => state.ToString()
    };
}
