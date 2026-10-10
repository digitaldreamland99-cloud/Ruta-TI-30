namespace RutaTI30.Infrastructure;

/// <summary>
/// Ubicación de los datos de usuario en el perfil del usuario (independiente de la carpeta de instalación).
/// Por defecto: %LOCALAPPDATA%/RutaTI30. Las pruebas pueden inyectar un directorio temporal.
/// </summary>
public sealed class AppPaths
{
    public string DataDirectory { get; }
    public string DatabasePath => Path.Combine(DataDirectory, "ruta-ti-30.db");
    public string BackupDirectory => Path.Combine(DataDirectory, "backups");
    public string LogPath => Path.Combine(DataDirectory, "logs", "rutati30.log");

    public AppPaths(string? dataDirectory = null)
    {
        DataDirectory = dataDirectory ?? DefaultDataDirectory();
    }

    public static string DefaultDataDirectory()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(local))
            local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        return Path.Combine(local, "RutaTI30");
    }

    /// <summary>Crea los directorios necesarios si no existen.</summary>
    public void EnsureDirectories()
    {
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(BackupDirectory);
        Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
    }
}
