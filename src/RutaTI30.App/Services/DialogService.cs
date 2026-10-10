using System.Windows;
using Microsoft.Win32;

namespace RutaTI30.App.Services;

/// <summary>Mensajes, confirmaciones y selector de archivo. Envuelve los diálogos nativos de WPF.</summary>
public sealed class DialogService
{
    public void Info(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void Error(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>Confirmación explícita para operaciones importantes (restaurar, reiniciar temporizador, etc.).</summary>
    public bool Confirm(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning,
            MessageBoxResult.No);
        return result == MessageBoxResult.Yes;
    }

    /// <summary>Selector de archivo para guardar; null si el usuario cancela.</summary>
    public string? SaveFile(string filter, string defaultFileName)
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
    public string? OpenFile(string filter)
    {
        var dialog = new OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
