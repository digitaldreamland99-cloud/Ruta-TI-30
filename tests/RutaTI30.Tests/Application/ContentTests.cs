using RutaTI30.Application.Content;
using Xunit;

namespace RutaTI30.Tests.Application;

public class ContentTests
{
    [Fact]
    public void Lecciones_TreintaDiasUnicos()
    {
        var lessons = LessonCatalog.Instance.Lessons;
        Assert.Equal(30, lessons.Count);
        Assert.Equal(Enumerable.Range(1, 30), lessons.Select(l => l.Day).OrderBy(d => d));
        Assert.Equal(30, lessons.Select(l => l.Id).Distinct().Count());
    }

    [Fact]
    public void Lecciones_EstructuraPedagogicaCompleta()
    {
        foreach (var l in LessonCatalog.Instance.Lessons)
        {
            Assert.False(string.IsNullOrWhiteSpace(l.Objective), $"{l.Id}: sin objetivo");
            Assert.InRange(l.Difficulty, 1, 5);
            Assert.True(l.EstimatedMinutes >= 30, $"{l.Id}: duración insuficiente");
            Assert.NotEmpty(l.Prerequisites);
            Assert.NotEmpty(l.BeginnerExplanation);
            Assert.NotEmpty(l.Concepts);
            Assert.NotEmpty(l.WorkplaceExample);
            Assert.NotEmpty(l.Procedure);
            Assert.NotEmpty(l.CommonMistakes);
            Assert.NotEmpty(l.GuidedExercise);
            Assert.NotEmpty(l.IndependentExercise);
            Assert.NotEmpty(l.Summary);
            Assert.NotEmpty(l.ReviewRecommendations);
            Assert.NotEmpty(l.ApprovalCriteria);
            Assert.True(l.BeginnerExplanation.Length >= 200, $"{l.Id}: explicación demasiado breve");
        }
    }

    [Fact]
    public void Lecciones_QuestionIdsUnicosPorLeccion()
    {
        foreach (var l in LessonCatalog.Instance.Lessons)
        {
            Assert.Equal(l.Questions.Count, l.Questions.Select(q => q.Id).Distinct().Count());
        }
    }

    [Fact]
    public void BancoDeEntrevistas_CamposObligatorios()
    {
        var bank = InterviewBank.Instance;
        Assert.True(bank.Questions.Count >= 10);
        foreach (var q in bank.Questions)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.Statement), $"{q.Id}: sin enunciado");
            Assert.False(string.IsNullOrWhiteSpace(q.ModelAnswer), $"{q.Id}: sin respuesta orientativa");
            Assert.False(string.IsNullOrWhiteSpace(q.Explanation), $"{q.Id}: sin explicación");
            Assert.NotEmpty(q.KeyConcepts);
            Assert.NotEmpty(q.CommonMistakes);
            Assert.NotEmpty(q.EvaluationCriteria);
        }
        Assert.Equal(bank.Questions.Count, bank.Questions.Select(q => q.Id).Distinct().Count());
    }
}
