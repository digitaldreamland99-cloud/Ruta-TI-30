using Microsoft.Data.Sqlite;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;

namespace RutaTI30.Tests;

/// <summary>Base de datos temporal por prueba: se crea y elimina en el sistema temporal.</summary>
public sealed class TempDb : IDisposable
{
    public string DirectoryPath { get; }
    public AppDb Db { get; }

    public TempDb()
    {
        DirectoryPath = Path.Combine(
            Path.GetTempPath(), "rutati30-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);

        Db = new AppDb(Path.Combine(DirectoryPath, "test.db"));
        Db.Initialize();

        CleanupBackupsAt = Path.Combine(DirectoryPath, "backups");
        if (Directory.Exists(CleanupBackupsAt))
            Directory.Delete(CleanupBackupsAt, true);
    }

    public void Dispose()
    {
        try { Db.Dispose(); }
        catch { }

        try
        {
            var dir = DirectoryPath;
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    // Backups creados por BackupService dentro del directorio temporal de la prueba;
    // TempDb los elimina con el resto del directorio.
    public string CleanupBackupsAt { get; }
}

/// <summary>Reloj controlable para pruebas de temporización.</summary>
public sealed class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } =
        new(2026, 1, 5, 12, 0, 0, DateTimeKind.Utc);

    public DateTime LocalNow => UtcNow.ToLocalTime();

    public void Advance(TimeSpan delta) => UtcNow += delta;
    public void AdvanceMinutes(int minutes) => Advance(TimeSpan.FromMinutes(minutes));
}

/// <summary>Fábrica de lecciones mínimas para pruebas de servicios.</summary>
public static class TestLessons
{
    public static Lesson Create(
        string id = "leccion-test",
        int questionCount = 4,
        int correctAnswers = 3,
        string topic = "TemaA",
        int? fixedCorrectIndex = null)
    {
        var questions = new List<EvaluationQuestion>();
        for (int i = 0; i < questionCount; i++)
        {
            int correctIndex = fixedCorrectIndex ?? 0;
            questions.Add(new EvaluationQuestion
            {
                Id = $"{id}-q{i}",
                Statement = $"Pregunta {i}",
                Options = new[] { "Opción correcta", "Opción incorrecta" },
                CorrectIndex = correctIndex,
                Explanation = "Explicación razonada.",
                Topic = i % 2 == 0 ? topic : "TemaB"
            });
        }

        var lesson = ContentFactory.L(
            1, LearningBlock.Fundamentos, "Lección de prueba", "Objetivo medible",
            1, 30, new[] { "Previo" }, "Explicación de prueba para principiantes.",
            new[] { ("Concepto", "Definición") }, "Ejemplo laboral de prueba.",
            new[] { "Paso 1", "Paso 2" }, new[] { "Error frecuente" },
            "Ejercicio guiado", "Ejercicio independiente",
            questions.ToArray(),
            "Resumen", "Repaso", new[] { "Referencia" }, "Aprobación ≥ 80 %",
            new[] { topic });

        if (lesson.Questions.Count > 0 && lesson.Questions[0].Options.Length > 0)
        {
            var opts = lesson.Questions[0].Options.ToList();
            opts[0] = opts[0] + " (correcta)";
            var arr = opts.ToArray();
            lesson = lesson with
            {
                Questions = lesson.Questions.Select((q, idx) =>
                    idx == 0 ? q with { Options = arr, CorrectIndex = 0 } : q).ToArray()
            };
        }

        if (fixedCorrectIndex.HasValue && lesson.Questions.Count > 0)
        {
            var opts = lesson.Questions[0].Options.ToList();
            opts[0] = opts[0] + " (correcta)";
            var arr = opts.ToArray();
            lesson = lesson with
            {
                Questions = lesson.Questions.Select((q, idx) =>
                    idx == 0 ? q with { Options = arr, CorrectIndex = fixedCorrectIndex.Value } : q).ToArray()
            };
        }

        return lesson;
    }
}
