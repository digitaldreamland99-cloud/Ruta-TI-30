using System.Windows;
using Microsoft.Win32;

namespace RutaTI30.App.Services;

/// <summary>Mensajes, confirmaciones y selector de archivo. Envuelve los diálogos nativos de WPF.
/// No es sellada y sus métodos son virtuales para permitir un doble de pruebas sin ventanas reales
/// (los ViewModels la reciben por inyección; en producción se registra la implementación de aquí).</summary>
public class DialogService
{
    public virtual void Info(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public virtual void Error(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>Confirmación explícita para operaciones importantes (restaurar, reiniciar temporizador, etc.).</summary>
    public virtual bool Confirm(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning,
            MessageBoxResult.No);
        return result == MessageBoxResult.Yes;
    }

    /// <summary>Selector de archivo para guardar; null si el usuario cancela.</summary>
    public virtual string? SaveFile(string filter, string defaultFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName,
            AddExtension = true
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <summary>Selector de archivo para abrir; null si el usuario cancela.</summary>
    public virtual string? OpenFile(string filter)
    {
        var dialog = new OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
