using System.Text.Json;
using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;

namespace RutaTI30.Application.Services;

/// <summary>Horario editable con valores iniciales del programa. El horario nunca bloquea el acceso al contenido.</summary>
public sealed class ScheduleService
{
    public const string ScheduleKey = "schedule.blocks";
    private readonly IPreferenceStore _preferences;

    public ScheduleService(IPreferenceStore preferences)
    {
        _preferences = preferences;
    }

    public IReadOnlyList<ScheduleBlock> GetSchedule()
    {
        var stored = _preferences.GetJson<List<ScheduleBlock>>(ScheduleKey);
        if (stored is { Count: > 0 } && Validate(stored).Count == 0)
            return stored;
        return DefaultSchedule();
    }

    public IReadOnlyList<string> Validate(IReadOnlyList<ScheduleBlock> blocks)
    {
        var errors = new List<string>();
        if (blocks is null || blocks.Count == 0)
        {
            errors.Add("El horario debe tener al menos un bloque.");
            return errors;
        }

        foreach (var b in blocks)
        {
            if (b.StartMinutes < 0 || b.StartMinutes > 24 * 60)
                errors.Add($"Bloque «{b.Label}»: hora de inicio fuera de rango.");
            if (b.EndMinutes <= b.StartMinutes)
                errors.Add($"Bloque «{b.Label}»: la hora final debe ser posterior a la inicial.");
            if (string.IsNullOrWhiteSpace(b.Label))
                errors.Add("Todos los bloques deben tener un nombre.");
        }

        var duplicatedIds = blocks.GroupBy(b => b.Id).Where(g => g.Count() > 1).Select(g => g.Key);
        foreach (var id in duplicatedIds)
            errors.Add($"Identificador duplicado en el horario: {id}.");

        return errors;
    }

    /// <summary>Guarda el horario solo si es válido; devuelve los errores sin guardar si no lo es.</summary>
    public IReadOnlyList<string> SaveSchedule(IReadOnlyList<ScheduleBlock> blocks)
    {
        var errors = Validate(blocks);
        if (errors.Count > 0) return errors;
        _preferences.SetJson(ScheduleKey, blocks.ToList());
        return errors;
    }

    /// <summary>Bloque activo a una hora local dada; null si cae fuera de horario.</summary>
    public ScheduleBlock? BlockAt(DateTime localTime)
    {
        int minutes = localTime.Hour * 60 + localTime.Minute;
        return GetSchedule()
            .FirstOrDefault(b => minutes >= b.StartMinutes && minutes < b.EndMinutes);
    }

    public static IReadOnlyList<ScheduleBlock> DefaultSchedule() => new List<ScheduleBlock>
    {
        new() { Id = 1, StartMinutes = 9 * 60, EndMinutes = 10 * 60, Kind = ScheduleKind.Aprendizaje, Label = "Aprendizaje principal" },
        new() { Id = 2, StartMinutes = 10 * 60, EndMinutes = 10 * 60 + 10, Kind = ScheduleKind.Descanso, Label = "Descanso" },
        new() { Id = 3, StartMinutes = 10 * 60 + 10, EndMinutes = 11 * 60, Kind = ScheduleKind.Comprension, Label = "Comprensión y ejemplos" },
        new() { Id = 4, StartMinutes = 11 * 60, EndMinutes = 11 * 60 + 15, Kind = ScheduleKind.Descanso, Label = "Descanso" },
        new() { Id = 5, StartMinutes = 11 * 60 + 15, EndMinutes = 12 * 60, Kind = ScheduleKind.PracticaGuiada, Label = "Práctica guiada" },
        new() { Id = 6, StartMinutes = 12 * 60, EndMinutes = 13 * 60, Kind = ScheduleKind.Comida, Label = "Comida" },
        new() { Id = 7, StartMinutes = 13 * 60, EndMinutes = 14 * 60, Kind = ScheduleKind.Laboratorio, Label = "Laboratorio" },
        new() { Id = 8, StartMinutes = 14 * 60, EndMinutes = 14 * 60 + 10, Kind = ScheduleKind.Descanso, Label = "Descanso" },
        new() { Id = 9, StartMinutes = 14 * 60 + 10, EndMinutes = 15 * 60, Kind = ScheduleKind.PracticaIndependiente, Label = "Práctica independiente" },
        new() { Id = 10, StartMinutes = 15 * 60, EndMinutes = 15 * 60 + 10, Kind = ScheduleKind.Descanso, Label = "Descanso" },
        new() { Id = 11, StartMinutes = 15 * 60 + 10, EndMinutes = 16 * 60, Kind = ScheduleKind.Repaso, Label = "Repaso o preparación laboral" }
    };
}
