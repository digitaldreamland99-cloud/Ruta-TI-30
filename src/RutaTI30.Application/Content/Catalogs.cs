using RutaTI30.Domain;

namespace RutaTI30.Application.Content;

/// <summary>Catálogo de escenarios simulados del laboratorio de soporte.</summary>
public interface ILabCatalog
{
    IReadOnlyList<LabScenario> Scenarios { get; }
    LabScenario? GetById(string id);
}

public sealed class LabCatalog : ILabCatalog
{
    public static readonly LabCatalog Instance = new(LabScenarios.All);
    private readonly IReadOnlyList<LabScenario> _scenarios;

    public LabCatalog(IReadOnlyList<LabScenario> scenarios) => _scenarios = scenarios;

    public IReadOnlyList<LabScenario> Scenarios => _scenarios;
    public LabScenario? GetById(string id) => _scenarios.FirstOrDefault(s => s.Id == id);
}

/// <summary>Banco de preguntas de entrevista con respuestas orientativas y criterios.</summary>
public interface IInterviewBank
{
    IReadOnlyList<InterviewQuestion> Questions { get; }
    InterviewQuestion? GetById(string id);
}

public sealed class InterviewBank : IInterviewBank
{
    public static readonly InterviewBank Instance = new(InterviewQuestions.All);
    private readonly IReadOnlyList<InterviewQuestion> _questions;

    public InterviewBank(IReadOnlyList<InterviewQuestion> questions) => _questions = questions;

    public IReadOnlyList<InterviewQuestion> Questions => _questions;
    public InterviewQuestion? GetById(string id) => _questions.FirstOrDefault(q => q.Id == id);
}
