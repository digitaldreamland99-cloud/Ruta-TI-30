using RutaTI30.Application.Content;
using Xunit;

namespace RutaTI30.Tests.Application;

public class GlossaryTests
{
    [Fact]
    public void Glosario_EntradasRealesConIdsUnicos()
    {
        var all = GlossaryCatalog.Instance.All;
        Assert.True(all.Count >= 55, $"Se esperaban al menos 55 entradas y hay {all.Count}");
        Assert.Equal(all.Count, all.Select(e => e.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(all.Count, all.Select(e => e.Term).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void Glosario_CamposObligatoriosCompletos()
    {
        foreach (var e in GlossaryCatalog.Instance.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(e.Term), $"{e.Id}: sin término");
            Assert.False(string.IsNullOrWhiteSpace(e.Category), $"{e.Id}: sin categoría");
            Assert.False(string.IsNullOrWhiteSpace(e.SimpleDefinition), $"{e.Id}: sin definición sencilla");
            Assert.False(string.IsNullOrWhiteSpace(e.TechnicalExplanation), $"{e.Id}: sin explicación técnica");
            Assert.False(string.IsNullOrWhiteSpace(e.WorkplaceExample), $"{e.Id}: sin ejemplo laboral");
            Assert.False(string.IsNullOrWhiteSpace(e.CommonMistake), $"{e.Id}: sin error frecuente");
            Assert.NotEmpty(e.RelatedTermIds);
            Assert.NotEmpty(e.RelatedLessonDays);
        }
    }

    [Fact]
    public void Glosario_LosEnlacesApuntanAContenidoExistente()
    {
        var catalog = GlossaryCatalog.Instance;
        foreach (var e in catalog.All)
        {
            foreach (var termId in e.RelatedTermIds)
                Assert.True(catalog.GetById(termId) is not null,
                    $"{e.Id}: el concepto relacionado «{termId}» no existe");

            foreach (var day in e.RelatedLessonDays)
            {
                Assert.InRange(day, 1, 30);
                Assert.True(LessonCatalog.Instance.GetByDay(day) is not null,
                    $"{e.Id}: la lección del día {day} no existe");
            }
        }
    }

    [Fact]
    public void Glosario_BusquedaInsensibleAMayusculasYacentes()
    {
        var catalog = GlossaryCatalog.Instance;

        var upper = catalog.Search("DNS", null);
        Assert.Contains(upper, e => e.Id == "dns");

        // «autenticacion» sin acento debe encontrar «Autenticación».
        var noAccent = catalog.Search("autenticacion", null);
        Assert.Contains(noAccent, e => e.Id == "autenticacion");

        var partial = catalog.Search("contrase", null);
        Assert.NotEmpty(partial);
    }

    [Fact]
    public void Glosario_FiltroPorCategoria()
    {
        var catalog = GlossaryCatalog.Instance;
        var redes = catalog.Search(null, "Redes");
        Assert.NotEmpty(redes);
        Assert.All(redes, e => Assert.Equal("Redes", e.Category));

        Assert.Empty(catalog.Search(null, "Categoria inexistente"));
    }

    [Fact]
    public void Glosario_SinFiltrosDevuelveTodoOrdenado()
    {
        var catalog = GlossaryCatalog.Instance;
        var all = catalog.Search(null, null);
        Assert.Equal(catalog.All.Count, all.Count);
        var sorted = all.Select(e => (e.Category.ToLowerInvariant(), e.Term.ToLowerInvariant())).ToList();
        var expected = sorted.OrderBy(x => x.Item1, StringComparer.Ordinal)
                             .ThenBy(x => x.Item2, StringComparer.Ordinal).ToList();
        Assert.Equal(expected, sorted);
    }

    [Fact]
    public void Glosario_CubreLasCategoriasDelPrograma()
    {
        var categories = GlossaryCatalog.Instance.Categories;
        foreach (var expected in new[]
                 {
                     "Computación y hardware", "Windows y diagnóstico", "Redes",
                     "Service Desk", "Seguridad e IAM"
                 })
            Assert.Contains(expected, categories);
    }
}
