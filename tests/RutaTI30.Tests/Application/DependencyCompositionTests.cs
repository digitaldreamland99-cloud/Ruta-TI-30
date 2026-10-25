using Microsoft.Extensions.DependencyInjection;
using RutaTI30.Application.Abstractions;
using RutaTI30.Application.Content;
using RutaTI30.Application.Services;
using RutaTI30.App.Services;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class DependencyCompositionTests
{
    [Fact]
    public void AppWiring_IGlossaryCatalogRegistered_GlossaryViewModelComposable()
    {
        // Reproduce el mismo registro que App.xaml.cs en un contenedor aislado.
        // No arranca WPF ni toca la ventana principal; solo comprueba que el contenedor
        // puede crear GlossaryViewModel con sus dependencias reales.
        var services = new ServiceCollection();

        services.AddSingleton<PreferenceStore>();
        services.AddSingleton<IPreferenceStore>(sp => sp.GetRequiredService<PreferenceStore>());
        services.AddSingleton<IGlossaryCatalog>(sp => GlossaryCatalog.Instance);

        services.AddSingleton<DialogService>();
        services.AddSingleton<ILessonCatalog>(LessonCatalog.Instance);
        services.AddSingleton<GlossaryViewModel>();

        var provider = services.BuildServiceProvider();

        var glossary = provider.GetRequiredService<IGlossaryCatalog>();
        Assert.NotNull(glossary);
        Assert.NotEmpty(glossary.All);
        Assert.NotEmpty(glossary.Categories);

        var vm = provider.GetRequiredService<GlossaryViewModel>();
        Assert.NotNull(vm);
        Assert.NotEmpty(vm.Categories);

        // Verifica que el catálogo inyectado es el real (no null).
        var casted = Assert.IsType<GlossaryCatalog>(glossary);
        Assert.Equal(glossary, casted.Instance);
    }

    [Fact]
    public void AppWiring_ICatalogasYServiciosBasicosComponibles()
    {
        var services = new ServiceCollection();

        services.AddSingleton<PreferenceStore>();
        services.AddSingleton<IPreferenceStore>(sp => sp.GetRequiredService<PreferenceStore>());

        services.AddSingleton<ILessonCatalog>(LessonCatalog.Instance);
        services.AddSingleton<ILabCatalog>(LabCatalog.Instance);
        services.AddSingleton<IInterviewBank>(InterviewBank.Instance);
        services.AddSingleton<IGlossaryCatalog>(sp => GlossaryCatalog.Instance);

        services.AddSingleton<EvaluationService>();
        services.AddSingleton<ProgressService>();
        services.AddSingleton<ScheduleService>();
        services.AddSingleton<DashboardService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<ThemeService>(sp => new ThemeService(System.Windows.Application.Current));

        services.AddSingleton<GlossaryViewModel>();
        services.AddSingleton<RoutesViewModel>();

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<ILessonCatalog>());
        Assert.NotNull(provider.GetRequiredService<ILabCatalog>());
        Assert.NotNull(provider.GetRequiredService<IInterviewBank>());
        Assert.NotNull(provider.GetRequiredService<IGlossaryCatalog>());

        Assert.NotNull(provider.GetRequiredService<GlossaryViewModel>());
        Assert.NotNull(provider.GetRequiredService<RoutesViewModel>());
    }

    [Fact]
    public void GlossaryCatalog_ContractoYDatosReales()
    {
        var catalog = GlossaryCatalog.Instance;

        Assert.NotEmpty(catalog.All);
        Assert.NotEmpty(catalog.Categories);

        var term = catalog.All[0];
        var byId = catalog.GetById(term.Id);
        Assert.NotNull(byId);
        Assert.Equal(term.Id, byId.Id);

        var search = catalog.Search(term.Term, null);
        Assert.Contains(search, e => e.Id == term.Id);

        var categoria = catalog.Search(null, term.Category);
        Assert.Contains(categoria, e => e.Id == term.Id);

        var filtrado = catalog.Search(term.Term, term.Category);
        Assert.Contains(filtrado, e => e.Id == term.Id);
    }

    [Fact]
    public void SpecializedRoutesCatalog_ContractoYDatosReales()
    {
        Assert.NotEmpty(SpecializationRoutes.All);

        var route = SpecializationRoutes.All[0];
        Assert.NotEmpty(route.Competencies);
        Assert.NotEmpty(route.LessonDays);
        Assert.NotEmpty(route.InterviewAreas);
        Assert.NotEmpty(route.EntryRoles);
    }

    [Fact]
    public void LessonCatalog_LessonsYDiasConsistentes()
    {
        var catalog = LessonCatalog.Instance;
        Assert.Equal(30, catalog.Lessons.Count);

        for (int day = 1; day <= 30; day++)
        {
            var lesson = catalog.GetByDay(day);
            Assert.NotNull(lesson);
            Assert.Equal(day, lesson.Day);
            Assert.NotEmpty(lesson.Questions);
        }
    }
}
