using System.Text.Json;

namespace RutaTI30.Application.Services;

/// <summary>Detalle JSON de un intento de evaluación: {"answers":{qid:indice|null},"failedTopics":[...]}.</summary>
public static class AttemptDetails
{
    public static string Serialize(IReadOnlyDictionary<string, int?> answers, IReadOnlyList<string> failedTopics)
    {
        var payload = new Dictionary<string, object?>
        {
            ["answers"] = answers,
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

    public static bool IsCorrect(string json, string questionId)
    {
        try
        {
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            if (doc.RootElement.TryGetProperty("answers", out var answers) &&
                answers.ValueKind == JsonValueKind.Object &&
                answers.TryGetProperty(questionId, out var value))
            {
                return value.ValueKind == JsonValueKind.Number;
            }
        }
        catch (JsonException)
        {
            // Detalle ilegible: se trata como no comprobado.
        }
        return false;
    }
}
