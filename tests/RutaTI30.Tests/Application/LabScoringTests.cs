using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using Xunit;

namespace RutaTI30.Tests.Application;

public class LabScoringTests
{
    [Fact]
    public void Scoring_AccionRecomendada_Puntua()
    {
        var scenario = LabCatalog.Instance.GetById("lab-01");
        Assert.NotNull(scenario);

        var score = LabScoring.Score(scenario!, new[] { "a1" });

        Assert.True(score.Points > 0);
        Assert.True(score.MaxPoints > 0);
        Assert.False(score.Passed); // una sola acción no alcanza el 70 %
    }

    [Fact]
    public void Scoring_AccionesInseguras_RestanYNoBajanDeCero()
    {
        var scenario = LabCatalog.Instance.GetById("lab-01");
        Assert.NotNull(scenario);

        var score = LabScoring.Score(scenario!, new[] { "a4", "a6" });

        Assert.Equal(0, score.Points);
        Assert.Equal(0, score.Percentage);
        Assert.NotEmpty(score.UnsafeActions);
        Assert.False(score.Passed);
    }

    [Fact]
    public void Scoring_TodasLasRecomendadas_Aprueba()
    {
        var scenario = LabCatalog.Instance.GetById("lab-01");
        Assert.NotNull(scenario);
        var recomendadas = scenario!.Actions.Where(a => a.ScoreWeight > 0).Select(a => a.Id).ToArray();

        var score = LabScoring.Score(scenario, recomendadas);

        Assert.Equal(score.MaxPoints, score.Points);
        Assert.Equal(100, score.Percentage);
        Assert.True(score.Passed);
        Assert.Empty(score.RecommendedMissed);
    }

    [Fact]
    public void Scoring_AccionDesconocida_SeIgnora()
    {
        var scenario = LabCatalog.Instance.GetById("lab-01");
        Assert.NotNull(scenario);

        var score = LabScoring.Score(scenario!, new[] { "no-existe" });

        Assert.Equal(0, score.Points);
    }

    [Fact]
    public void Catalogo_DiezEscenariosConCriterios()
    {
        Assert.Equal(10, LabCatalog.Instance.Scenarios.Count);
        foreach (var s in LabCatalog.Instance.Scenarios)
        {
            Assert.NotEmpty(s.Context);
            Assert.NotEmpty(s.Symptoms);
            Assert.NotEmpty(s.Actions);
            Assert.NotEmpty(s.Solution);
            Assert.NotEmpty(s.EscalationGuidance);
            Assert.NotEmpty(s.EvaluationCriteria);
            Assert.True(s.Actions.Any(a => a.ScoreWeight > 0), $"{s.Id}: sin acciones recomendadas");
        }
    }
}
