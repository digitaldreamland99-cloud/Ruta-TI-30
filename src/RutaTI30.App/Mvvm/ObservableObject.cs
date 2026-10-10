using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RutaTI30.App.Mvvm;

/// <summary>Base MVVM ligera (notificación de propiedad). Implementada localmente para no depender
/// de paquetes que no pueden verificarse en el entorno actual; ver DECISIONS.md D2.</summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
