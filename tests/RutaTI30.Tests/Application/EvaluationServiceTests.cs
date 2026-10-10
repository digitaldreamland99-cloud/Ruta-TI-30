using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using Xunit;

namespace RutaTI30.Tests.Application;

public class EvaluationServiceTests
{
    [Fact]
    public void Evaluate_TodasCorrectas_Obtiene100YAprueba()
    {
        var lesson = TestLessons.Create(questionCount: 4);
        var service = new EvaluationService();
        var seleccion = Enumerable.Repeat<int?>(0, lesson.Questions.Count).ToList();

        var result = service.Evaluate(lesson, seleccion);

        Assert.Equal(100, result.Score);
        Assert.True(result.Passed);
        Assert.Equal(0, result.UnansweredCount);
        Assert.Empty(result.FailedTopics);
    }

    [Fact]
    public void Evaluate_TresDeCuatro_Es75NoAprobado()
    {
        var lesson = TestLessons.Create(questionCount: 4);
        var service = new EvaluationService();
        var seleccion = new List<int?> { 0, 0, 0, 1 };

        var result = service.Evaluate(lesson, seleccion);

        Assert.Equal(75, result.Score);
        Assert.False(result.Passed);
        Assert.NotEmpty(result.FailedTopics);
    }

    [Fact]
    public void Evaluate_PreguntasSinResponder_CuentanComoFallidas()
    {
        var lesson = TestLessons.Create(questionCount: 3);
        var service = new EvaluationService();
        var seleccion = new List<int?> { 0, null, null };

        var result = service.Evaluate(lesson, seleccion);

        Assert.Equal(2, result.UnansweredCount);
        Assert.Equal(1, result.Correct);
        Assert.False(result.Passed);
    }

    [Fact]
    public void Evaluate_CantidadDistintaDeRespuestas_LanzaArgumentException()
    {
        var lesson = TestLessons.Create(questionCount: 3);
        var service = new EvaluationService();

        Assert.Throws<ArgumentException>(() => service.Evaluate(lesson, new List<int?> { 0 }));
    }

    [Fact]
    public void Evaluate_UmbralPersonalizado70_Aprueba75()
    {
        var lesson = TestLessons.Create(questionCount: 4);
        var service = new EvaluationService(70);
        var seleccion = new List<int?> { 0, 0, 0, 1 };

        var result = service.Evaluate(lesson, seleccion);

        Assert.True(result.Passed);
        Assert.Equal(70, service.Threshold);
    }

    [Fact]
    public void Evaluate_UmbralInvalidoUsa80PorDefecto()
    {
        var service = new EvaluationService(250);
        Assert.Equal(80, service.Threshold);
    }

    [Fact]
    public void ContenidoEmbebido_PreguntasTienenOpcionesValidas()
    {
        foreach (var lesson in LessonCatalog.Instance.Lessons)
        {
            Assert.True(lesson.Questions.Count >= 3, $"{lesson.Id} debe tener al menos 3 preguntas");
            foreach (var q in lesson.Questions)
            {
                Assert.True(q.Options.Count >= 2, $"{q.Id}: opciones insuficientes");
                Assert.InRange(q.CorrectIndex, 0, q.Options.Count - 1);
                Assert.False(string.IsNullOrWhiteSpace(q.Explanation), $"{q.Id}: sin explicación");
            }
        }
    }
}
