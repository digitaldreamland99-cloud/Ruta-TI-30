using System.Globalization;
using Microsoft.Data.Sqlite;

namespace RutaTI30.Infrastructure;

/// <summary>Formato y utilidades SQLite compartidas: fechas ISO-8601 UTC y parámetros seguros.</summary>
public static class DbFormat
{
    public const string UtcFormat = "yyyy-MM-ddTHH:mm:ssZ";

    public static string Utc(DateTime value)
    {
        var utc = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        return utc.ToString(UtcFormat, CultureInfo.InvariantCulture);
    }

    public static DateTime? ParseUtc(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : DateTime.ParseExact(value, UtcFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

    public static int BoolToInt(bool value) => value ? 1 : 0;

    public static bool IntToBool(int value) => value != 0;

    public static void Add(SqliteParameterCollection parameters, string name, object? value) =>
        parameters.AddWithValue(name, value ?? DBNull.Value);
}
