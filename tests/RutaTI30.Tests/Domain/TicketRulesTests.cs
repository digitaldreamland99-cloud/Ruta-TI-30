using RutaTI30.Domain;
using Xunit;

namespace RutaTI30.Tests.Domain;

public class TicketRulesTests
{
    [Theory]
    [InlineData(ImpactLevel.Alto, UrgencyLevel.Alta, PriorityLevel.P1)]
    [InlineData(ImpactLevel.Alto, UrgencyLevel.Media, PriorityLevel.P2)]
    [InlineData(ImpactLevel.Medio, UrgencyLevel.Alta, PriorityLevel.P2)]
    [InlineData(ImpactLevel.Alto, UrgencyLevel.Baja, PriorityLevel.P3)]
    [InlineData(ImpactLevel.Medio, UrgencyLevel.Media, PriorityLevel.P3)]
    [InlineData(ImpactLevel.Bajo, UrgencyLevel.Alta, PriorityLevel.P3)]
    [InlineData(ImpactLevel.Medio, UrgencyLevel.Baja, PriorityLevel.P4)]
    [InlineData(ImpactLevel.Bajo, UrgencyLevel.Media, PriorityLevel.P4)]
    [InlineData(ImpactLevel.Bajo, UrgencyLevel.Baja, PriorityLevel.P4)]
    public void CalculatePriority_CubreTodaLaMatriz(ImpactLevel impacto, UrgencyLevel urgencia, PriorityLevel esperado)
    {
        Assert.Equal(esperado, TicketRules.CalculatePriority(impacto, urgencia));
    }

    [Theory]
    [InlineData(TicketState.Nuevo, TicketState.EnAtencion, true)]
    [InlineData(TicketState.Nuevo, TicketState.Escalado, true)]
    [InlineData(TicketState.EnAtencion, TicketState.Resuelto, true)]
    [InlineData(TicketState.EnAtencion, TicketState.Cerrado, false)] // no se cierra sin resolver
    [InlineData(TicketState.Resuelto, TicketState.Cerrado, true)]
    [InlineData(TicketState.Cerrado, TicketState.EnAtencion, false)] // cerrado es final
    [InlineData(TicketState.Nuevo, TicketState.Nuevo, false)]        // misma estado no es transición
    [InlineData(TicketState.Escalado, TicketState.Cerrado, false)]
    public void CanTransition_ValidaTransiciones(TicketState desde, TicketState hacia, bool esperado)
    {
        Assert.Equal(esperado, TicketRules.CanTransition(desde, hacia));
    }

    [Fact]
    public void Validate_RechazaReporterVacio()
    {
        var errors = TicketRules.Validate("   ", "Descripción válida con más de diez caracteres");
        Assert.Contains(errors, e => e.Contains("usuario afectado", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_RechazaDescripcionCorta()
    {
        var errors = TicketRules.Validate("Ana", "corta");
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Validate_AceptaDatosValidos()
    {
        var errors = TicketRules.Validate("Ana Pérez", "El equipo no enciende desde esta mañana.");
        Assert.Empty(errors);
    }
}
