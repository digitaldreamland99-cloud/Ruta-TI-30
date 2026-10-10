using Microsoft.Extensions.Logging;

namespace RutaTI30.App.Services;

/// <summary>Registro técnico local mínimo: escribe errores en un archivo del perfil del usuario.
/// No envía nada a ningún servicio externo.</summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _path;
    private readonly object _sync = new();

    public FileLoggerProvider(string path)
    {
        _path = path;
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

    public void Dispose()
    {
    }

    internal void Write(string category, LogLevel level, string message, Exception? exception)
    {
        try
        {
            lock (_sync)
            {
                File.AppendAllText(_path,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {category}: {message}" +
                    (exception is null ? Environment.NewLine : Environment.NewLine + exception + Environment.NewLine));
            }
        }
        catch (IOException)
        {
            // Un fallo de escritura de log no debe impedir el uso de la aplicación.
        }
        catch (UnauthorizedAccessException)
        {
            // Misma razón que el IOException: si el archivo de registro no es escribible,
            // la aplicación continúa; el usuario verá el error en la interfaz.
        }
    }

    private sealed class FileLogger : ILogger
    {
        private readonly FileLoggerProvider _provider;
        private readonly string _category;

        public FileLogger(FileLoggerProvider provider, string category)
        {
            _provider = provider;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            _provider.Write(_category, logLevel, formatter(state, exception), exception);
        }
    }
}
