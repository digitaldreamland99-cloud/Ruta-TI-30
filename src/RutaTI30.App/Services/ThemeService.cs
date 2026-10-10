using System.Windows;

namespace RutaTI30.App.Services;

/// <summary>Cambia el diccionario de recursos del tema (oscuro por defecto, claro opcional).</summary>
public sealed class ThemeService
{
    public const string Dark = "Dark";
    public const string Light = "Light";
    private readonly Application _application;
    private string _current = Dark;

    public ThemeService(Application application) => _application = application;

    public string Current => _current;

    public void Apply(string theme)
    {
        var resource = theme == Light ? "Themes/Light.xaml" : "Themes/Dark.xaml";
        var dictionaries = _application.Resources.MergedDictionaries;
        for (int i = 0; i < dictionaries.Count; i++)
        {
            var source = dictionaries[i].Source?.ToString() ?? string.Empty;
            if (source.EndsWith("Dark.xaml", StringComparison.OrdinalIgnoreCase) ||
                source.EndsWith("Light.xaml", StringComparison.OrdinalIgnoreCase))
            {
                dictionaries[i] = new ResourceDictionary { Source = new Uri(resource, UriKind.Relative) };
                _current = theme == Light ? Light : Dark;
                return;
            }
        }
    }
}
