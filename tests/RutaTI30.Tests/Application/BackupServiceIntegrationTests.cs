using RutaTI30.Application.Abstractions;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using RutaTI30.App;
using Xunit;

namespace RutaTI30.Tests.Application;

public class BackupServiceIntegrationTests : IDisposable
{
    private readonly TempDb _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public void BackupService_WithExplicitChangeNotifier_NotifiesAfterRestore()
    {
        // Arrange
        var notifier = new DbChangedNotifier();
        var backupService = new BackupService(_db.Db, Path.Combine(_db.DirectoryPath, "backups"), notifier);
        var store = new LearningStore(_db.Db);

        store.SaveProgress(new LessonProgress
        {
            LessonId = "leccion-01",
            State = LessonState.Completed,
            StartedUtc = DateTime.UtcNow
        });

        var backupPath = backupService.CreateBackup();

        // Act
        var notified = false;
        notifier.DataChanged += () => notified = true;

        var result = backupService.Restore(backupPath);

        // Assert
        Assert.True(result.Success);
        Assert.True(notified, "El observador no recibió la notificación de cambio de base después de restaurar.");
    }

    [Fact]
    public void BackupService_WithObserverThatReloads_OnDataChangedReloadsFromFreshStore()
    {
        // Arrange
        var notifier = new DbChangedNotifier();
        var backupService = new BackupService(_db.Db, Path.Combine(_db.DirectoryPath, "backups"), notifier);
        var store = new LearningStore(_db.Db);

        store.SaveProgress(new LessonProgress
        {
            LessonId = "leccion-01",
            State = LessonState.InProgress,
            StartedUtc = DateTime.UtcNow
        });

        var backupPath = backupService.CreateBackup();

        store.SaveProgress(new LessonProgress
        {
            LessonId = "leccion-01",
            State = LessonState.Mastered
        });

        // Simulate a ViewModel that caches some data and reloads on notification
        var subjectReloaded = new SubjectReloaded(notifier, _db.Db);

        // Act
        subjectReloaded.TriggerReloadOnNextNotification();
        var result = backupService.Restore(backupPath);

        // Assert
        Assert.True(result.Success);
        Assert.True(subjectReloaded.ObservedReload, "El sujeto que observa el cambio de base no recargó.");
        var reloadedStore = new LearningStore(_db.Db);
        var reloadedProgress = reloadedStore.GetAllProgress();
        Assert.Single(reloadedProgress);
        Assert.Equal(LessonState.Mastered, reloadedProgress[0].State);
    }

    private sealed class SubjectReloaded
    {
        private readonly IDbStateChanged _notifier;
        private readonly AppDb _db;

        public bool ObservedReload { get; private set; }

        public SubjectReloaded(IDbStateChanged notifier, AppDb db)
        {
            _notifier = notifier;
            _db = db;
        }

        public void TriggerReloadOnNextNotification()
        {
            _notifier.DataChanged += OnDataChanged;
        }

        private void OnDataChanged()
        {
            ObservedReload = true;
            var store = new LearningStore(_db);
            var progress = store.GetAllProgress();
            Assert.NotNull(progress);
            Assert.False(progress.Count == 0, "No hay datos después de recargar.");
        }
    }
}
