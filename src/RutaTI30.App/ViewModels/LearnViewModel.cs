using System.Collections.ObjectModel;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.Mvvm;
using RutaTI30.App.Services;
using RutaTI30.Domain;

namespace RutaTI30.App.ViewModels;

/// <summary>Día de la ruta en la lista lateral.</summary>
public sealed class DayItemVm
{
    public int Day { get; init; }
    public string Title { get; init; } = string.Empty;
    public string BlockName { get; init; } = string.Empty;
    public string StateLabel { get; init; } = string.Empty;
}

/// <summary>Concepto de vocabulario (término → definición).</summary>
public sealed record ConceptVm(string Term, string Definition);

/// <summary>Opción de una pregunta de evaluación.</summary>
public sealed class OptionVm
{
    public int Index { get; init; }
    public string Text { get; init; } = string.Empty;
}

/// <summary>Pregunta mostrada en la evaluación.</summary>
public sealed class QuestionVm : ObservableObject
{
    public string Statement { get; init; } = string.Empty;
    public string Explanation { get; init; } = string.Empty;
    public ObservableCollection<OptionVm> Options { get; } = new();

    private OptionVm? _selected;
    public OptionVm? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    private string _feedback = string.Empty;
    public string Feedback
    {
        get => _feedback;
        set => SetProperty(ref _feedback, value);
    }
}

/// <summary>Ruta de aprendizaje de 30 días: contenido, estados y evaluaciones.</summary>
public sealed class LearnViewModel : ObservableObject, INavigableModule
{
    private readonly ILessonCatalog _catalog;
    private readonly ILearningStore _store;
    private readonly EvaluationService _evaluation;
    private readonly DialogService _dialogs;
    private readonly IClock _clock;

    public ObservableCollection<DayItemVm> Days { get; } = new();

    private DayItemVm? _selectedDay;
    public DayItemVm? SelectedDay
    {
        get => _selectedDay;
        set
        {
            if (SetProperty(ref _selectedDay, value) && value is not null)
                LoadLesson(value.Day);
        }
    }

    // --- Lección seleccionada ---
    private bool _hasLesson;
    public bool HasLesson { get => _hasLesson; private set => SetProperty(ref _hasLesson, value); }
    private string _lessonTitle = string.Empty;
    private string _lessonObjective = string.Empty;
    private string _lessonMeta = string.Empty;
    private string _lessonStateText = string.Empty;
    private string _explanation = string.Empty;
    private string _workplace = string.Empty;
    private string _guided = string.Empty;
    private string _independent = string.Empty;
    private string _summary = string.Empty;
    private string _review = string.Empty;
    private string _approval = string.Empty;
    private string _lastScoreText = string.Empty;

    public string LessonTitle { get => _lessonTitle; private set => SetProperty(ref _lessonTitle, value); }
    public string LessonObjective { get => _lessonObjective; private set => SetProperty(ref _lessonObjective, value); }
    public string LessonMeta { get => _lessonMeta; private set => SetProperty(ref _lessonMeta, value); }
    public string LessonStateText { get => _lessonStateText; private set => SetProperty(ref _lessonStateText, value); }
    public string Explanation { get => _explanation; private set => SetProperty(ref _explanation, value); }
    public string Workplace { get => _workplace; private set => SetProperty(ref _workplace, value); }
    public string Guided { get => _guided; private set => SetProperty(ref _guided, value); }
    public string Independent { get => _independent; private set => SetProperty(ref _independent, value); }
    public string Summary { get => _summary; private set => SetProperty(ref _summary, value); }
    public string Review { get => _review; private set => SetProperty(ref _review, value); }
    public string Approval { get => _approval; private set => SetProperty(ref _approval, value); }
    public string LastScoreText { get => _lastScoreText; private set => SetProperty(ref _lastScoreText, value); }

    public ObservableCollection<string> Prerequisites { get; } = new();
    public ObservableCollection<ConceptVm> Concepts { get; } = new();
    public ObservableCollection<string> Procedure { get; } = new();
    public ObservableCollection<string> Mistakes { get; } = new();
    public ObservableCollection<string> References { get; } = new();
    public ObservableCollection<string> AttemptHistory { get; } = new();

    // --- Evaluación ---
    public ObservableCollection<QuestionVm> Questions { get; } = new();
    private bool _evaluationVisible;
    private string _evaluationResult = string.Empty;
    public bool EvaluationVisible { get => _evaluationVisible; private set => SetProperty(ref _evaluationVisible, value); }
    public string EvaluationResult { get => _evaluationResult; private set => SetProperty(ref _evaluationResult, value); }

    public AsyncRelayCommand StartLessonCommand { get; }
    public AsyncRelayCommand CompleteLessonCommand { get; }
    public AsyncRelayCommand SubmitEvaluationCommand { get; }
    public AsyncRelayCommand ToggleEvaluationCommand { get; }

    private Lesson? _currentLesson;

    public LearnViewModel(ILessonCatalog catalog, ILearningStore store, EvaluationService evaluation,
        DialogService dialogs, IClock clock)
    {
        _catalog = catalog;
        _store = store;
        _evaluation = evaluation;
        _dialogs = dialogs;
        _clock = clock;

        StartLessonCommand = new AsyncRelayCommand(() => ChangeStateAsync(LessonState.InProgress));
        CompleteLessonCommand = new AsyncRelayCommand(() => ChangeStateAsync(LessonState.Completed));
        SubmitEvaluationCommand = new AsyncRelayCommand(SubmitEvaluationAsync);
        ToggleEvaluationCommand = new AsyncRelayCommand(ToggleEvaluationAsync);
    }

    public Task RefreshAsync()
    {
        try
        {
            var states = _store.GetAllProgress()
                .ToDictionary(p => p.LessonId, p => p.State);
            int current = _selectedDay?.Day ?? 1;
            Days.Clear();
            foreach (var day in _catalog.GetDays())
            {
                var state = states.TryGetValue($"leccion-{day.Day:D2}", out var s) ? s : LessonState.NotStarted;
                Days.Add(new DayItemVm
                {
                    Day = day.Day,
                    Title = day.Title,
                    BlockName = day.Block.ToString(),
                    StateLabel = StateLabel(state)
                });
            }
            _selectedDay = Days.FirstOrDefault(d => d.Day == current) ?? Days.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedDay));
            if (_selectedDay is not null)
                LoadLesson(_selectedDay.Day);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Ruta de aprendizaje", "No se pudo cargar la ruta: " + ex.Message);
        }
        return Task.CompletedTask;
    }

    private void LoadLesson(int day)
    {
        try
        {
            var lesson = _catalog.GetByDay(day);
            if (lesson is null)
            {
                HasLesson = false;
                return;
            }
            _currentLesson = lesson;
            HasLesson = true;
            EvaluationVisible = false;
            EvaluationResult = string.Empty;
            Questions.Clear();

            LessonTitle = $"Día {lesson.Day} · {lesson.Title}";
            LessonObjective = lesson.Objective;
            LessonMeta = $"Dificultad {lesson.Difficulty}/5 · {lesson.EstimatedMinutes} min · {lesson.Block}";
            Explanation = lesson.BeginnerExplanation;
            Workplace = lesson.WorkplaceExample;
            Guided = lesson.GuidedExercise;
            Independent = lesson.IndependentExercise;
            Summary = lesson.Summary;
            Review = lesson.ReviewRecommendations;
            Approval = lesson.ApprovalCriteria;

            Fill(Prerequisites, lesson.Prerequisites);
            Fill(Procedure, lesson.Procedure);
            Fill(Mistakes, lesson.CommonMistakes);
            Fill(References, lesson.References);
            Concepts.Clear();
            foreach (var c in lesson.Concepts)
                Concepts.Add(new ConceptVm(c.Key, c.Value));

            RefreshStateDisplay(lesson.Id);
        }
        catch (Exception ex)
        {
            _dialogs.Error("Lección", "No se pudo abrir la lección: " + ex.Message);
        }
    }

    private void RefreshStateDisplay(string lessonId)
    {
        var progress = _store.GetProgress(lessonId);
        var state = progress?.State ?? LessonState.NotStarted;
        LessonStateText = StateLabel(state);
        LastScoreText = progress?.LastScore is double score
            ? $"Último puntaje: {score:0.#} %"
            : "Aún sin evaluaciones";

        AttemptHistory.Clear();
        foreach (var attempt in _store.GetAttempts(lessonId).TakeLast(10))
        {
            AttemptHistory.Add(
                $"{attempt.AttemptedUtc.ToLocalTime():dd/MM/yyyy HH:mm} · {attempt.Score:0.#} % · " +
                (attempt.Passed ? "aprobado" : "no aprobado"));
        }
    }

    private async Task ChangeStateAsync(LessonState target)
    {
        if (_currentLesson is null) return;
        try
        {
            var progress = _store.GetProgress(_currentLesson.Id) ?? new LessonProgress
            {
                LessonId = _currentLesson.Id
            };

            if (target == LessonState.InProgress && progress.State == LessonState.NotStarted)
            {
                progress.State = LessonState.InProgress;
                progress.StartedUtc = _clock.UtcNow;
            }
            else if (target == LessonState.Completed &&
                     progress.State is LessonState.InProgress or LessonState.Completed)
            {
                // Completar la lectura nunca otorga el dominio.
                progress.State = LessonState.Completed;
                progress.CompletedUtc ??= _clock.UtcNow;
            }
            else if (target == LessonState.InProgress && progress.State != LessonState.NotStarted)
            {
                _dialogs.Info("Lección", "Esta lección ya está en curso o completada.");
            }
            else if (target == LessonState.Completed && progress.State == LessonState.NotStarted)
            {
                _dialogs.Info("Lección",
                    "Primero marca la lección como «En curso» y realiza los ejercicios antes de completarla.");
            }
            else if (progress.State == LessonState.Mastered)
            {
                _dialogs.Info("Lección", "Esta lección ya está dominada mediante evaluación.");
            }

            _store.SaveProgress(progress);
            RefreshStateDisplay(_currentLesson.Id);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Lección", "No se pudo actualizar el estado: " + ex.Message);
        }
    }

    private Task ToggleEvaluationAsync()
    {
        if (_currentLesson is null) return Task.CompletedTask;
        if (!EvaluationVisible)
            BuildQuestions(_currentLesson);
        EvaluationVisible = !EvaluationVisible;
        return Task.CompletedTask;
    }

    private void BuildQuestions(Lesson lesson)
    {
        Questions.Clear();
        foreach (var q in lesson.Questions)
        {
            var vm = new QuestionVm { Statement = q.Statement, Explanation = q.Explanation };
            for (int i = 0; i < q.Options.Count; i++)
                vm.Options.Add(new OptionVm { Index = i, Text = q.Options[i] });
            Questions.Add(vm);
        }
        EvaluationResult = string.Empty;
    }

    private async Task SubmitEvaluationAsync()
    {
        if (_currentLesson is null) return;
        try
        {
            if (Questions.Count != _currentLesson.Questions.Count)
            {
                _dialogs.Error("Evaluación", "Las preguntas no se cargaron correctamente. Vuelve a abrirlas.");
                return;
            }

            var answers = new Dictionary<string, int?>();
            var correct = new Dictionary<string, bool>();
            var selected = new List<int?>();
            for (int i = 0; i < Questions.Count; i++)
            {
                var index = Questions[i].Selected?.Index;
                selected.Add(index);
                answers[_currentLesson.Questions[i].Id] = index;
                correct[_currentLesson.Questions[i].Id] = index == _currentLesson.Questions[i].CorrectIndex;
            }

            var unanswered = selected.Count(s => s is null);
            if (unanswered == Questions.Count)
            {
                _dialogs.Error("Evaluación", "Responde al menos una pregunta antes de enviar.");
                return;
            }

            var result = _evaluation.Evaluate(_currentLesson, selected);
            var attempt = new EvaluationAttempt
            {
                LessonId = _currentLesson.Id,
                AttemptedUtc = _clock.UtcNow,
                Score = result.Score,
                Passed = result.Passed,
                DetailsJson = AttemptDetails.SerializeAttempt(answers, correct, result.FailedTopics)
            };
            _store.AddAttempt(attempt);

            // Actualizar estado según resultados (dominio solo mediante evaluación aprobada).
            var progress = _store.GetProgress(_currentLesson.Id) ?? new LessonProgress
            {
                LessonId = _currentLesson.Id
            };
            if (progress.State == LessonState.NotStarted)
            {
                progress.State = LessonState.InProgress;
                progress.StartedUtc ??= _clock.UtcNow;
            }
            if (result.Passed && progress.State == LessonState.Completed)
            {
                progress.State = LessonState.Mastered;
                progress.MasteredUtc = _clock.UtcNow;
            }
            else if (result.Passed && progress.State == LessonState.InProgress)
            {
                progress.State = LessonState.Completed;
                progress.CompletedUtc ??= _clock.UtcNow;
            }
            progress.LastScore = result.Score;
            _store.SaveProgress(progress);

            // Feedback por pregunta con la solución razonada.
            for (int i = 0; i < Questions.Count; i++)
            {
                var q = _currentLesson.Questions[i];
                var chosen = selected[i];
                bool correct = chosen == q.CorrectIndex;
                Questions[i].Feedback = (correct ? "✔ Correcta. " : "✘ Incorrecta. ") + q.Explanation;
            }

            EvaluationResult =
                $"Puntaje: {result.Score:0.#} % · {result.Correct} de {result.Total} correctas · " +
                (result.Passed
                    ? (progress.State == LessonState.Mastered
                        ? "¡Dominio alcanzado!"
                        : "Aprobado: marca la lección como completada para aspirar al dominio.")
                    : $"No aprobado (umbral {_evaluation.Threshold:0} %). Revisa las soluciones y repite el intento; " +
                      "se conserva el historial.") +
                (result.FailedTopics.Count > 0
                    ? $" · Temas a repasar: {string.Join(", ", result.FailedTopics)}"
                    : string.Empty);

            RefreshStateDisplay(_currentLesson.Id);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            _dialogs.Error("Evaluación", "No se pudo evaluar: " + ex.Message);
        }
    }

    private static void Fill(ObservableCollection<string> target, IReadOnlyList<string> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(item);
    }

    private static string StateLabel(LessonState state) => state switch
    {
        LessonState.InProgress => "En curso",
        LessonState.Completed => "Completado",
        LessonState.Mastered => "Dominado",
        _ => "No iniciado"
    };
}
