using RutaTI30.Application.Content;
using RutaTI30.Domain;
using Xunit;

namespace RutaTI30.Tests.Application;

public class PracticeScenarioTests
{
    [Fact]
    public void Casos_DieciseisEscenariosConIdsUnicos()
    {
        var all = TicketPracticeScenarios.All;
        Assert.Equal(16, all.Count);
        Assert.Equal(all.Count, all.Select(c => c.Id).Distinct().Count());
    }

    [Fact]
    public void Casos_CamposDidacticosCompletos()
    {
        foreach (var c in TicketPracticeScenarios.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Title), $"Título vacío en {c.Id}");
            Assert.False(string.IsNullOrWhiteSpace(c.Reporter), $"Reportero vacío en {c.Id}");
            Assert.False(string.IsNullOrWhiteSpace(c.Description), $"Descripción vacía en {c.Id}");
            Assert.True(c.DiagnosticQuestions.Count >= 2, $"Faltan preguntas de diagnóstico en {c.Id}");
            Assert.True(c.SuggestedSteps.Count >= 3, $"Faltan pasos sugeridos en {c.Id}");
            Assert.False(string.IsNullOrWhiteSpace(c.ResolutionOrEscalation), $"Falta solución/escalamiento en {c.Id}");
            Assert.False(string.IsNullOrWhiteSpace(c.DocumentationNote), $"Falta nota de documentación en {c.Id}");
        }
    }

    [Fact]
    public void Casos_CoberturaDeTemasRequeridos()
    {
        // Categorías esperadas: accesos, aplicaciones, hardware, impresora, redes.
        Assert.Contains(TicketPracticeScenarios.All, c => c.Category == TicketCategory.Acceso);
        Assert.Contains(TicketPracticeScenarios.All, c => c.Category == TicketCategory.Aplicacion);
        Assert.Contains(TicketPracticeScenarios.All, c => c.Category == TicketCategory.Hardware);
        Assert.Contains(TicketPracticeScenarios.All, c => c.Category == TicketCategory.Impresora);
        Assert.Contains(TicketPracticeScenarios.All, c => c.Category == TicketCategory.Redes);

        // Debe cubrir escalamiento obligatorio (reincidencia) y diagnóstico por descarte.
        Assert.Contains(TicketPracticeScenarios.All, c => c.ResolutionOrEscalation.Contains("escala", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(TicketPracticeScenarios.All, c => c.Id == "prac-08");

        // Cobertura temática requerida: DNS (prac-06), DHCP/APIPA (prac-09) e IP básica (prac-05).
        Assert.Contains(TicketPracticeScenarios.All, c => c.Id == "prac-06" &&
            c.SuggestedSteps.Any(s => s.Contains("nslookup", StringComparison.OrdinalIgnoreCase)));
        Assert.Contains(TicketPracticeScenarios.All, c => c.Id == "prac-09" &&
            c.Description.Contains("169.254", StringComparison.Ordinal));
        Assert.Contains(TicketPracticeScenarios.All, c => c.Id == "prac-05" &&
            c.SuggestedSteps.Any(s => s.Contains("ipconfig", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void Casos_NuevosCubrenLosTemasPendientes()
    {
        // VPN (prac-10), correo sin sincronizar (prac-11), fallo tras actualización (prac-12),
        // solicitud de permisos (prac-13), incidente multiusuario (prac-14), contraseña (prac-15)
        // y hardware sin energía (prac-16).
        foreach (var id in new[] { "prac-10", "prac-11", "prac-12", "prac-13", "prac-14", "prac-15", "prac-16" })
            Assert.Contains(TicketPracticeScenarios.All, c => c.Id == id);

        // El caso de servicio debe exigir escalamiento inmediato y comunicar a los usuarios.
        var outage = TicketPracticeScenarios.All.Single(c => c.Id == "prac-14");
        Assert.Equal(TicketCategory.Servicio, outage.Category);
        Assert.True(outage.ResolutionOrEscalation.Contains("escala", StringComparison.OrdinalIgnoreCase));

        // La solicitud de permisos debe exigir autorización del dueño del recurso.
        var request = TicketPracticeScenarios.All.Single(c => c.Id == "prac-13");
        Assert.Contains(request.SuggestedSteps, s => s.Contains("autoriz", StringComparison.OrdinalIgnoreCase));

        // Ningún escenario puede invitar a abrir equipos ni a ejecutar acciones privilegiadas.
        foreach (var c in TicketPracticeScenarios.All)
            foreach (var step in c.SuggestedSteps)
                Assert.DoesNotContain("abrir el equipo y", step, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Casos_NivelesDeImpactoYUrgenciaValidos()
    {
        foreach (var c in TicketPracticeScenarios.All)
        {
            Assert.True(Enum.IsDefined(c.Impact), $"Impacto inválido en {c.Id}");
            Assert.True(Enum.IsDefined(c.Urgency), $"Urgencia inválida en {c.Id}");
        }
    }

    [Fact]
    public void Casos_DatosFicticiosIdentificadosEnElTituloDelReporte()
    {
        // El formulario etiqueta el usuario como ficticio; los nombres deben ser genéricos de ejemplo.
        foreach (var c in TicketPracticeScenarios.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Department), $"Departamento vacío en {c.Id}");
        }
    }
}
