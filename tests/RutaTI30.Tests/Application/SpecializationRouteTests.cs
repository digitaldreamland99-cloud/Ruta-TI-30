using RutaTI30.Application.Content;
using Xunit;

namespace RutaTI30.Tests.Application;

public class SpecializationRouteTests
{
    [Fact]
    public void Rutas_CincoItinerariosConIdsUnicos()
    {
        var all = SpecializationRoutes.All;
        Assert.Equal(5, all.Count);
        Assert.Equal(all.Count, all.Select(r => r.Id).Distinct().Count());
        Assert.Equal(all.Count, all.Select(r => r.Title).Distinct().Count());
    }

    [Fact]
    public void Rutas_CamposObligatoriosCompletos()
    {
        foreach (var r in SpecializationRoutes.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(r.Title), $"{r.Id}: sin título");
            Assert.False(string.IsNullOrWhiteSpace(r.Focus), $"{r.Id}: sin enfoque");
            Assert.False(string.IsNullOrWhiteSpace(r.Prerequisites), $"{r.Id}: sin prerrequisitos");
            Assert.NotEmpty(r.Competencies);
            Assert.NotEmpty(r.LessonDays);
            Assert.NotEmpty(r.PracticeIds);
            Assert.NotEmpty(r.InterviewAreas);
            Assert.False(string.IsNullOrWhiteSpace(r.EntryRoles), $"{r.Id}: sin puestos relacionados");
        }
    }

    [Fact]
    public void Rutas_LosEnlacesApuntanAContenidoExistente()
    {
        foreach (var r in SpecializationRoutes.All)
        {
            foreach (var day in r.LessonDays)
            {
                Assert.InRange(day, 1, 30);
                Assert.True(LessonCatalog.Instance.GetByDay(day) is not null,
                    $"{r.Id}: la lección del día {day} no existe");
            }

            foreach (var practiceId in r.PracticeIds)
                Assert.Contains(TicketPracticeScenarios.All, s => s.Id == practiceId);

            var areas = InterviewBank.Instance.Questions
                .Select(q => q.Area)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var area in r.InterviewAreas)
                Assert.Contains(area, areas);
        }
    }

    [Fact]
    public void Rutas_LasCincoAreasRequeridasEstanCubiertas()
    {
        var ids = SpecializationRoutes.All.Select(r => r.Id).ToHashSet();
        foreach (var id in new[] { "ruta-a", "ruta-b", "ruta-c", "ruta-d", "ruta-e" })
            Assert.Contains(id, ids);
    }
}
