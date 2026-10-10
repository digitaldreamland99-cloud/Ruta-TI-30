using System.Text.Json;
using Microsoft.Data.Sqlite;
using RutaTI30.Application.Abstractions;

namespace RutaTI30.Infrastructure;

/// <summary>Preferencias clave/valor persistidas (JSON para estructuras como el horario).</summary>
public sealed class PreferenceStore : IPreferenceStore
{
    private readonly AppDb _db;

    public PreferenceStore(AppDb db) => _db = db;

    public string? Get(string key)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT value FROM preferences WHERE key = @key;";
        cmd.Parameters.AddWithValue("@key", key);
        return cmd.ExecuteScalar() as string;
    }

    public void Set(string key, string value)
    {
        using var connection = _db.OpenConnection();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO preferences (key, value) VALUES (@key, @value)
ON CONFLICT(key) DO UPDATE SET value = excluded.value;";
        cmd.Parameters.AddWithValue("@key", key);
        cmd.Parameters.AddWithValue("@value", value);
        cmd.ExecuteNonQuery();
    }

    public int GetInt(string key, int defaultValue)
    {
        var raw = Get(key);
        return int.TryParse(raw, out var value) ? value : defaultValue;
    }

    public void SetInt(string key, int value) => Set(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    public T? GetJson<T>(string key)
    {
        var raw = Get(key);
        if (string.IsNullOrWhiteSpace(raw)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(raw);
        }
        catch (JsonException)
        {
            // Valor corrupto: se ignora y el llamador usará el valor por defecto.
            return default;
        }
    }

    public void SetJson<T>(string key, T value) =>
        Set(key, JsonSerializer.Serialize(value));
}
