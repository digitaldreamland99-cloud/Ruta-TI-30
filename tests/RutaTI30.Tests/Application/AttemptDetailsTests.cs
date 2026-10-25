using System.Collections.Generic;
using System.Text.Json;
using RutaTI30.Application.Services;
using Xunit;
// No aplica: la clase no depende de WPF ni de App.

namespace RutaTI30.Tests.Application;

public class AttemptDetailsTests
{
    /// <summary>Formato actual de un intento evaluado: answers + correct por pregunta + failedTopics.</summary>
    private static string MakeAttempt(
        IReadOnlyDictionary<string, int?> answers,
        IReadOnlyDictionary<string, bool> correct,
        IReadOnlyList<string>? failedTopics = null)
    {
        return AttemptDetails.SerializeAttempt(answers, correct, failedTopics ?? Array.Empty<string>());
    }

    private static string MakeLegacy(string answersBlock = "\"q1\":0", bool includeAnswersCorrectLegacy = false)
    {
        var payload = new Dictionary<string, object?>
        {
            ["answers"] = new Dictionary<string, int?> { ["q1"] = 0 }
        };
        if (includeAnswersCorrectLegacy)
            payload["answersCorrect"] = new Dictionary<string, bool> { ["q1"] = true };
        return JsonSerializer.Serialize(payload);
    }

    [Fact]
    public void IsCorrect_SinRespuestaRegistrada_RetornaFalse()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = null },
            new Dictionary<string, bool> { ["q1"] = false });

        Assert.False(AttemptDetails.IsCorrect(json, "q1"));
    }

    [Fact]
    public void IsCorrect_ConRespuestaCorrectaRegistrada_RetornaTrue()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 0 },
            new Dictionary<string, bool> { ["q1"] = true });

        Assert.True(AttemptDetails.IsCorrect(json, "q1"));
    }

    [Fact]
    public void IsCorrect_ConRespuestaIncorrectaRegistrada_RetornaFalse()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 1 },
            new Dictionary<string, bool> { ["q1"] = false });

        Assert.False(AttemptDetails.IsCorrect(json, "q1"));
    }

    [Fact]
    public void IsCorrect_PreguntaInexistente_RetornaFalse()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 0 },
            new Dictionary<string, bool> { ["q1"] = true });

        Assert.False(AttemptDetails.IsCorrect(json, "q-inexistente"));
    }

    [Fact]
    public void IsCorrect_JsonVacio_RetornaFalse()
    {
        Assert.False(AttemptDetails.IsCorrect("{}", "q1"));
    }

    [Fact]
    public void IsCorrect_JsonNuloOWhitespace_RetornaFalse()
    {
        Assert.False(AttemptDetails.IsCorrect(null, "q1"));
        Assert.False(AttemptDetails.IsCorrect(string.Empty, "q1"));
        Assert.False(AttemptDetails.IsCorrect("   ", "q1"));
    }

    [Fact]
    public void IsCorrect_JsonMalformado_RetornaFalse()
    {
        Assert.False(AttemptDetails.IsCorrect("{corrupto", "q1"));
        Assert.False(AttemptDetails.IsCorrect("{\"answers\":", "q1"));
    }

    [Fact]
    public void IsCorrect_JsonConAnswersNoObject_RetornaFalse()
    {
        var json = "{\"answers\":[]}";
        Assert.False(AttemptDetails.IsCorrect(json, "q1"));
    }

    [Fact]
    public void IsCorrect_JsonConRespuestaString_RetornaFalse()
    {
        var json = "{\"answers\":{\"q1\":\"0\"}}";
        Assert.False(AttemptDetails.IsCorrect(json, "q1"));
    }

    [Fact]
    public void IsCorrect_IntentosNuevosDistinguenCorrectoDeIncorrecto()
    {
        var jsonCorrecto = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 0 },
            new Dictionary<string, bool> { ["q1"] = true });

        var jsonIncorrecto = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 1 },
            new Dictionary<string, bool> { ["q1"] = false });

        Assert.True(AttemptDetails.IsCorrect(jsonCorrecto, "q1"));
        Assert.False(AttemptDetails.IsCorrect(jsonIncorrecto, "q1"));
    }

    [Fact]
    public void IsCorrect_IntentosSinCampoCorrectNoSeMarcanComoAcierto()
    {
        // Un intento antiguo sin registro de corrección no debe interpretarse como acierto.
        var jsonSinCorrect = MakeLegacy();

        Assert.False(AttemptDetails.IsCorrect(jsonSinCorrect, "q1"));
        Assert.False(AttemptDetails.HasExplicitCorrectness(jsonSinCorrect));
    }

    [Fact]
    public void IsCorrect_LegacyAnswersCorrectTrueSeMantieneComoAciertoDocumentado()
    {
        // Algunos datos históricos pueden tener "answersCorrect". Un true explícito se conserva
        // como acierto documentado; un false o ausente no.
        var jsonLegacyTrue = MakeLegacy(includeAnswersCorrectLegacy: true);

        Assert.True(AttemptDetails.IsCorrect(jsonLegacyTrue, "q1"));
        Assert.True(AttemptDetails.HasExplicitCorrectness(jsonLegacyTrue));
    }

    [Fact]
    public void HasExplicitCorrectness_DistingueIntentosEvaluadosDeLegacySinCorrect()
    {
        var jsonEvaluado = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 0 },
            new Dictionary<string, bool> { ["q1"] = true });

        Assert.True(AttemptDetails.HasExplicitCorrectness(jsonEvaluado));
        Assert.False(AttemptDetails.HasExplicitCorrectness(MakeLegacy()));
    }

    [Fact]
    public void FailedTopics_DesdeJson_ValoresValidos()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = null, ["q2"] = 0 },
            new Dictionary<string, bool> { ["q1"] = false, ["q2"] = true },
            new[] { "TemaA", "TemaB", "TemaA" });

        var topics = AttemptDetails.FailedTopics(json);

        Assert.Equal(2, topics.Count);
        Assert.Contains("TemaA", topics);
        Assert.Contains("TemaB", topics);
    }

    [Fact]
    public void FailedTopics_JsonVacio_DevuelveListaVacia()
    {
        Assert.Empty(AttemptDetails.FailedTopics("{}"));
    }

    [Fact]
    public void FailedTopics_JsonNuloOWhitespace_DevuelveListaVacia()
    {
        Assert.Empty(AttemptDetails.FailedTopics(null));
        Assert.Empty(AttemptDetails.FailedTopics(string.Empty));
        Assert.Empty(AttemptDetails.FailedTopics("   "));
    }

    [Fact]
    public void FailedTopics_JsonMalformado_DevuelveListaVacia()
    {
        Assert.Empty(AttemptDetails.FailedTopics("{corrupto"));
        Assert.Empty(AttemptDetails.FailedTopics("{\"failedTopics\":"));
    }

    [Fact]
    public void FailedTopics_JsonSinFailedTopics_DevuelveListaVacia()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q1"] = 0 },
            new Dictionary<string, bool> { ["q1"] = true });

        Assert.Empty(AttemptDetails.FailedTopics(json));
    }

    [Fact]
    public void FailedTopics_EtiquetasCoincidentesConTopicDePregunta()
    {
        var json = MakeAttempt(
            new Dictionary<string, int?> { ["q-servicio"] = 1 },
            new Dictionary<string, bool> { ["q-servicio"] = false },
            new[] { "Servicio de Windows" });

        var topics = AttemptDetails.FailedTopics(json);
        Assert.Contains("Servicio de Windows", topics);

        // La corrección de cada pregunta es independiente de la etiqueta de tema.
        Assert.False(AttemptDetails.IsCorrect(json, "q-servicio"));
    }
}
