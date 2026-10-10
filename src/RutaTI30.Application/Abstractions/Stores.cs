using RutaTI30.Domain;

namespace RutaTI30.Application.Abstractions;

/// <summary>Reloj inyectable: toda medición de tiempo pasa por aquí (no por ticks de UI).</summary>
public interface IClock
{
    DateTime UtcNow { get; }
    DateTime LocalNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime LocalNow => DateTime.Now;
}

/// <summary>Almacén del progreso de lecciones y evaluaciones.</summary>
public interface ILearningStore
{
    IReadOnlyList<LessonProgress> GetAllProgress();
    LessonProgress? GetProgress(string lessonId);
    void SaveProgress(LessonProgress progress);
    IReadOnlyList<EvaluationAttempt> GetAttempts(string? lessonId = null);
    void AddAttempt(EvaluationAttempt attempt);
}

/// <summary>Almacén de sesiones de estudio y estado del temporizador.</summary>
public interface ISessionStore
{
    void SaveSession(StudySession session);
    StudySession? GetSession(long id);
    IReadOnlyList<StudySession> GetSessions(DateTime? fromUtc = null);
    int GetTotalEffectiveSeconds();
    TimerStateRow GetTimerState();
    void SaveTimerState(TimerStateRow state);
}

/// <summary>Almacén de tickets con historial (operaciones múltiples transaccionales).</summary>
public interface ITicketStore
{
    void AddWithHistory(Ticket ticket, TicketHistoryEntry creationEntry);
    void Update(Ticket ticket);
    void AddHistory(TicketHistoryEntry entry);
    Ticket? Get(string id);
    IReadOnlyList<Ticket> List(TicketState? stateFilter = null, string? search = null);
    IReadOnlyList<TicketHistoryEntry> GetHistory(string ticketId);
    int CountByState(TicketState state);
    int CountAll();
}

/// <summary>Almacén de entrevistas y postulaciones.</summary>
public interface IJobStore
{
    long AddApplication(JobApplication application);
    void UpdateApplication(JobApplication application);
    JobApplication? GetApplication(long id);
    IReadOnlyList<JobApplication> ListApplications();
    /// <summary>Elimina la postulación y sus seguimientos en una sola transacción. No falla si el id no existe.</summary>
    void DeleteApplication(long id);
    void AddFollowUp(FollowUp followUp);
    IReadOnlyList<FollowUp> GetFollowUps(long applicationId);
    void AddInterviewAttempt(InterviewAttempt attempt);
    IReadOnlyList<InterviewAttempt> GetInterviewAttempts(string? questionId = null);
}

/// <summary>Almacén de intentos de laboratorio.</summary>
public interface ILabStore
{
    long AddAttempt(LabAttempt attempt);
    void UpdateAttempt(LabAttempt attempt);
    IReadOnlyList<LabAttempt> GetAttempts(string? scenarioId = null);
}

/// <summary>Preferencias clave/valor del usuario (JSON para estructuras).</summary>
public interface IPreferenceStore
{
    string? Get(string key);
    void Set(string key, string value);
    int GetInt(string key, int defaultValue);
    void SetInt(string key, int value);
    T? GetJson<T>(string key);
    void SetJson<T>(string key, T value);
}
