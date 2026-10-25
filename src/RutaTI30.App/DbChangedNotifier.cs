namespace RutaTI30.App;

/// <summary>Implementación simple de <see cref="RutaTI30.Application.Abstractions.IDbStateChanged"/>.</summary>
public sealed class DbChangedNotifier : RutaTI30.Application.Abstractions.IDbStateChanged
{
    public event Action? DataChanged;

    public void Raise() => DataChanged?.Invoke();
}
