using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Infrastructure;

public class LearningStoreTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly LearningStore _store;

    public LearningStoreTests()
    {
        _store = new LearningStore(_db.Db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public void SaveProgress_YGet_SobrevivenAlReabrir()
    {
        _store.SaveProgress(new LessonProgress
        {
            LessonId = "leccion-01",
            State = LessonState.Mastered,
            StartedUtc = new DateTime(2026, 1, 5, 10, 0, 0, DateTimeKind.Utc),
            CompletedUtc = new DateTime(2026, 1, 5, 11, 0, 0, DateTimeKind.Utc),
            MasteredUtc = new DateTime(2026, 1, 5, 12, 0, 0, DateTimeKind.Utc),
            LastScore = 90
        });

        // Nueva instancia de AppDb sobre el mismo archivo = "reabrir la aplicación".
        var reapert = new AppDb(_db.Db.DatabasePath);
        var store2 = new LearningStore(reapert);
        var cargado = store2.GetProgress("leccion-01");

        Assert.NotNull(cargado);
        Assert.Equal(LessonState.Mastered, cargado!.State);
        Assert.Equal(90, cargado.LastScore);
        Assert.Equal(DateTimeKind.Utc, cargado.MasteredUtc!.Value.Kind);
        reapert.Dispose();
    }

    [Fact]
    public void SaveProgress_ActualizaSinDuplicar()
    {
        _store.SaveProgress(new LessonProgress { LessonId = "leccion-01", State = LessonState.InProgress });
        _store.SaveProgress(new LessonProgress { LessonId = "leccion-01", State = LessonState.Completed });

        var todos = _store.GetAllProgress();

        Assert.Single(todos);
        Assert.Equal(LessonState.Completed, todos[0].State);
    }

    [Fact]
    public void GetProgress_Inexistente_DevuelveNull()
    {
        Assert.Null(_store.GetProgress("no-existe"));
    }

    [Fact]
    public void AddAttempt_GuardaHistorialOrdenado()
    {
        _store.AddAttempt(new EvaluationAttempt { LessonId = "leccion-01", AttemptedUtc = DateTime.UtcNow, Score = 50, Passed = false, DetailsJson = "{}" });
        _store.AddAttempt(new EvaluationAttempt { LessonId = "leccion-01", AttemptedUtc = DateTime.UtcNow, Score = 100, Passed = true, DetailsJson = "{}" });
        _store.AddAttempt(new EvaluationAttempt { LessonId = "leccion-02", AttemptedUtc = DateTime.UtcNow, Score = 80, Passed = true, DetailsJson = "{}" });

        var deLeccion1 = _store.GetAttempts("leccion-01");
        var todos = _store.GetAttempts();

        Assert.Equal(2, deLeccion1.Count);
        Assert.Equal(3, todos.Count);
        Assert.True(deLeccion1[0].Id < deLeccion1[1].Id); // orden cronológico de inserción
        Assert.Equal(100, deLeccion1[1].Score);
    }
}
