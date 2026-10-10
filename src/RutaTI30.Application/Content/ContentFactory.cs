using RutaTI30.Domain;

namespace RutaTI30.Application.Content;

/// <summary>Fábrica para declarar lecciones y preguntas de forma compacta sin perder campos obligatorios.</summary>
public static class ContentFactory
{
    public static EvaluationQuestion Q(string id, string statement, string[] options, int correct,
        string explanation, string topic)
    {
        if (correct < 0 || correct >= options.Length)
            throw new ArgumentException($"Opción correcta inválida en la pregunta {id}.");
        return new EvaluationQuestion
        {
            Id = id,
            Statement = statement,
            Options = options,
            CorrectIndex = correct,
            Explanation = explanation,
            Topic = topic
        };
    }

    public static Lesson L(
        int day,
        LearningBlock block,
        string title,
        string objective,
        int difficulty,
        int estimatedMinutes,
        string[] prerequisites,
        string beginnerExplanation,
        (string Term, string Definition)[] concepts,
        string workplaceExample,
        string[] procedure,
        string[] commonMistakes,
        string guidedExercise,
        string independentExercise,
        EvaluationQuestion[] questions,
        string summary,
        string reviewRecommendations,
        string[] references,
        string approvalCriteria,
        string[] topics)
    {
        return new Lesson
        {
            Id = $"leccion-{day:D2}",
            Day = day,
            Block = block,
            Title = title,
            Objective = objective,
            Difficulty = difficulty,
            EstimatedMinutes = estimatedMinutes,
            Prerequisites = prerequisites,
            BeginnerExplanation = beginnerExplanation,
            Concepts = concepts.Select(c => new KeyValuePair<string, string>(c.Term, c.Definition)).ToList(),
            WorkplaceExample = workplaceExample,
            Procedure = procedure,
            CommonMistakes = commonMistakes,
            GuidedExercise = guidedExercise,
            IndependentExercise = independentExercise,
            Questions = questions,
            Summary = summary,
            ReviewRecommendations = reviewRecommendations,
            References = references,
            ApprovalCriteria = approvalCriteria,
            Topics = topics
        };
    }
}
