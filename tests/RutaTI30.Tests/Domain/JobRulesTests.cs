using RutaTI30.Domain;
using Xunit;

namespace RutaTI30.Tests.Domain;

public class JobRulesTests
{
    private static JobApplication Valid()
    {
        var now = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        return new JobApplication
        {
            Company = "Contoso",
            Role = "Soporte L1",
            DiscoveredUtc = now,
            AppliedUtc = now.AddDays(1),
            Url = "https://empleos.example/vacante-1"
        };
    }

    [Fact]
    public void PostulacionValida_NoProduceErrores()
    {
        Assert.Empty(JobRules.Validate(Valid()));
    }

    [Fact]
    public void EmpresaVacia_SeRechaza()
    {
        var app = Valid();
        app.Company = "   ";
        var errors = JobRules.Validate(app);
        Assert.Contains(errors, e => e.Contains("empresa"));
    }

    [Fact]
    public void PuestoVacio_SeRechaza()
    {
        var app = Valid();
        app.Role = "";
        var errors = JobRules.Validate(app);
        Assert.Contains(errors, e => e.Contains("puesto"));
    }

    [Theory]
    [InlineData("ftp://empleos.example/v")]
    [InlineData("empleos.example/v")]
    [InlineData("javascript:alert(1)")]
    public void EnlaceSinHttpSeRechaza(string url)
    {
        var app = Valid();
        app.Url = url;
        Assert.NotEmpty(JobRules.Validate(app));
    }

    [Fact]
    public void EnlaceVacio_SePermite()
    {
        var app = Valid();
        app.Url = "";
        Assert.Empty(JobRules.Validate(app));
    }

    [Fact]
    public void PostulacionAntesDeDescubrimiento_SeRechaza()
    {
        var app = Valid();
        app.AppliedUtc = app.DiscoveredUtc!.Value.AddDays(-2);
        var errors = JobRules.Validate(app);
        Assert.Contains(errors, e => e.Contains("postulación"));
    }

    [Fact]
    public void EntrevistaAntesDePostulacion_SeRechaza()
    {
        var app = Valid();
        app.InterviewUtc = app.AppliedUtc!.Value.AddDays(-1);
        var errors = JobRules.Validate(app);
        Assert.Contains(errors, e => e.Contains("entrevista"));
    }

    [Fact]
    public void SinFechas_SePermite()
    {
        var app = Valid();
        app.DiscoveredUtc = null;
        app.AppliedUtc = null;
        app.InterviewUtc = null;
        Assert.Empty(JobRules.Validate(app));
    }

    [Fact]
    public void HttpSinTambienSeAcepta()
    {
        var app = Valid();
        app.Url = "http://intranet.local/vacante";
        Assert.Empty(JobRules.Validate(app));
    }
}
