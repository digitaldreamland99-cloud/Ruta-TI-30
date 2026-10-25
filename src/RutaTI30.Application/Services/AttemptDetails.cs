using System.Text.Json;

namespace RutaTI30.Application.Services;

/// <summary>
/// Detalle JSON de un intento de evaluación.
/// Formato actual: {"answers":{qid:indice|null},"correct":{qid:true/false},"failedTopics":[...]}
/// Los intentos antiguos pueden tener la estructura anterior (sin "correct"). Esas entradas siguen
/// siendo legibles, pero no se pueden interpretar como acierto: se tratan como "no verificado".
/// </summary>
public static class AttemptDetails
{
    /// <summary>Versión compacta usada por escenarios que ya evaluaron correctamente por pregunta.</summary>
    public static string Serialize(IReadOnlyDictionary<string, int?> answers, IReadOnlyList<string> failedTopics)
    {
        var payload = new Dictionary<string, object?>
        {
            ["answers"] = answers,
            ["failedTopics"] = failedTopics.Distinct().ToList()
        };
        return JsonSerializer.Serialize(payload);
    }

    /// <summary>Serialización para intentos de evaluación que necesitan distinguir acierto de error.</summary>
    public static string SerializeAttempt(
        IReadOnlyDictionary<string, int?> answers,
        IReadOnlyDictionary<string, bool> correct,
        IReadOnlyList<string> failedTopics)
    {
        var payload = new Dictionary<string, object?>
        {
            ["answers"] = answers,
            ["correct"] = correct,
            ["failedTopics"] = failedTopics.Distinct().ToList()
        };
        return JsonSerializer.Serialize(payload);
    }

    public static IReadOnlyList<string> FailedTopics(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            if (doc.RootElement.TryGetProperty("failedTopics", out var arr) &&
                arr.ValueKind == JsonValueKind.Array)
            {
                return arr.EnumerateArray()
                    .Select(e => e.GetString() ?? string.Empty)
                    .Where(s => s.Length > 0)
                    .Distinct()
                    .ToList();
            }
        }
        catch (JsonException)
        {
            // Detalle ilegible: no debe romper el cálculo del progreso.
        }
        return Array.Empty<string>();
    }

    /// <summary>
    /// Devuelve true solo si el intento registra explícitamente que la respuesta fue correcta.
    /// Una respuesta incorrecta registrada, una respuesta sin contestar o un intento antiguo sin
    /// campo "correct" devuelven false. Esto evita que "hay respuesta registrada" se interprete
    /// como "respuesta correcta".
    /// </summary>
    public static bool IsCorrect(string json, string questionId)
    {
        try
        {
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            if (doc.RootElement.TryGetProperty("correct", out var correct) &&
                correct.ValueKind == JsonValueKind.Object &&
                correct.TryGetProperty(questionId, out var value))
            {
                if (value.ValueKind == JsonValueKind.True)
                    return true;
                if (value.ValueKind == JsonValueKind.False)
                    return false;
            }

            // Intentos antiguos pueden registrar la respuesta correcta explícitamente como
            // "answersCorrect" en lugar de "correct". Es una señal útil, pero ambigua: un true
            // ahí significa "la opción seleccionada era la correcta", no "se registró una respuesta".
            if (doc.RootElement.TryGetProperty("answersCorrect", out var legacy) &&
                legacy.ValueKind == JsonValueKind.Object &&
                legacy.TryGetProperty(questionId, out var legacyValue) &&
                legacyValue.ValueKind == JsonValueKind.True)
            {
                return true;
            }
        }
        catch (JsonException)
        {
            // Detalle ilegible: se trata como no comprobado.
        }
        return false;
    }

    /// <summary>Indica si un intento tiene información explícita de corrección por pregunta.</summary>
    public static bool HasExplicitCorrectness(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            var root = doc.RootElement;
            if (root.TryGetProperty("correct", out var correct) && correct.ValueKind == JsonValueKind.Object)
                return true;
            if (root.TryGetProperty("answersCorrect", out var legacy) && legacy.ValueKind == JsonValueKind.Object)
                return true;
        }
        catch (JsonException)
        {
        }
        return false;
    }
}
