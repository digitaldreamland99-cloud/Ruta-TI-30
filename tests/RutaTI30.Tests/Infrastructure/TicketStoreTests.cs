using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Infrastructure;

public class TicketStoreTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly TicketStore _store;
    private readonly DateTime _ahora = new DateTime(2026, 1, 5, 9, 0, 0, DateTimeKind.Utc);

    public TicketStoreTests()
    {
        _store = new TicketStore(_db.Db);
    }

    public void Dispose() => _db.Dispose();

    private Ticket NuevoTicket(string id = "INC-1", string reporter = "Ana Torres",
        string description = "El equipo de la sala 3 no enciende desde esta mañana.")
    {
        return new Ticket
        {
            Id = id,
            CreatedUtc = _ahora,
            UpdatedUtc = _ahora,
            Reporter = reporter,
            Department = "Contabilidad",
            Description = description,
            Category = TicketCategory.Hardware,
            Impact = ImpactLevel.Medio,
            Urgency = UrgencyLevel.Alta,
            Priority = TicketRules.CalculatePriority(ImpactLevel.Medio, UrgencyLevel.Alta),
            State = TicketState.Nuevo
        };
    }

    [Fact]
    public void AddWithHistory_CreaTicketYEHistorialTransaccional()
    {
        var ticket = NuevoTicket();
        _store.AddWithHistory(ticket, new TicketHistoryEntry
        {
            TicketId = ticket.Id,
            AtUtc = _ahora,
            Action = "Alta del ticket",
            ToState = TicketState.Nuevo
        });

        var guardado = _store.Get("INC-1");
        var historial = _store.GetHistory("INC-1");

        Assert.NotNull(guardado);
        Assert.Equal("Ana Torres", guardado!.Reporter);
        Assert.Equal(PriorityLevel.P2, guardado.Priority);
        Assert.Single(historial);
        Assert.Equal("Alta del ticket", historial[0].Action);
    }

    [Fact]
    public void AddWithHistory_SiFallaElHistorial_NoSeCreaElTicket()
    {
        var ticket = NuevoTicket("INC-2");
        var historialInvalido = new TicketHistoryEntry
        {
            TicketId = "OTRO-999", // clave foránea inexistente
            AtUtc = _ahora,
            Action = "Alta"
        };

        Assert.ThrowsAny<Exception>(() => _store.AddWithHistory(ticket, historialInvalido));
        Assert.Null(_store.Get("INC-2"));
        Assert.Equal(0, _store.CountAll());
    }

    [Fact]
    public void List_BusquedaLiteralNoTrataComodinesCmoComodines()
    {
        _store.AddWithHistory(NuevoTicket("INC-1", "Ana Torres", "El equipo de la sala 3 con % de núcleos."), new TicketHistoryEntry { TicketId = "INC-1", AtUtc = _ahora, Action = "Alta" });
        _store.AddWithHistory(NuevoTicket("INC-2", "Luis Mena", "Impresora con _ y cola sin trabajos."), new TicketHistoryEntry { TicketId = "INC-2", AtUtc = _ahora, Action = "Alta" });

        var porPorcentaje = _store.List(null, "%");
        var porGuionBajo = _store.List(null, "_");
        var porAmbos = _store.List(null, "%_");
        var porTextoNormal = _store.List(null, "impresora");
        var porEspacio = _store.List(null, "  ");

        Assert.Empty(porPorcentaje);
        Assert.Empty(porGuionBajo);
        Assert.Empty(porAmbos);
        Assert.Single(porTextoNormal);
        Assert.Empty(porEspacio);
    }

    [Theory]
    [InlineData("%")]
    [InlineData("_")]
    [InlineData("%_")]
    [InlineData("_%")]
    [InlineData(" % ")]
    [InlineData("a%b_c")]
    [InlineData("normal")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("¡#¿?")]
    public void List_BusquedaLiteralRespetaTextoEscritoPorElUsuario(string search)
    {
        _store.AddWithHistory(NuevoTicket("INC-1", "Ana Torres", "Reporte con la palabra clave."), new TicketHistoryEntry { TicketId = "INC-1", AtUtc = _ahora, Action = "Alta" });

        var resultados = _store.List(null, search);

        bool contieneNormal = search.Contains("clave", StringComparison.OrdinalIgnoreCase);

        if (contieneNormal)
            Assert.Single(resultados);
        else
            Assert.Empty(resultados);
    }

    [Fact]
    public void List_BusquedaNormal_CoincideSoloEnTextoLiteral()
    {
        _store.AddWithHistory(NuevoTicket("INC-1", "Ana Torres", "El equipo de la sala 3 no enciende."), new TicketHistoryEntry { TicketId = "INC-1", AtUtc = _ahora, Action = "Alta" });
        _store.AddWithHistory(NuevoTicket("INC-2", "Luis Mena", "Reinicio de servicios en el servidor."), new TicketHistoryEntry { TicketId = "INC-2", AtUtc = _ahora, Action = "Alta" });

        var coincidencia = _store.List(null, "servicio");
        var sinCoincidencia = _store.List(null, "impresora");
        var mayorQue = _store.List(null, ">");

        Assert.Single(coincidencia);
        Assert.Empty(sinCoincidencia);
        Assert.Empty(mayorQue);
    }

    [Fact]
    public void List_FiltraPorEstadoYTexto()
    {
        _store.AddWithHistory(NuevoTicket("INC-1", "Ana Torres", "El equipo de la sala 3 no enciende desde esta mañana."), new TicketHistoryEntry { TicketId = "INC-1", AtUtc = _ahora, Action = "Alta" });
        _store.AddWithHistory(NuevoTicket("INC-2", "Luis Mena", "La impresora del piso 2 no responde a los trabajos."), new TicketHistoryEntry { TicketId = "INC-2", AtUtc = _ahora, Action = "Alta" });

        var todos = _store.List();
        var porTexto = _store.List(null, "impresora");
        var porEstadoNuevo = _store.List(TicketState.Nuevo);
        var porEstadoCerrado = _store.List(TicketState.Cerrado);

        Assert.Equal(2, todos.Count);
        Assert.Single(porTexto);
        Assert.Equal(2, porEstadoNuevo.Count);
        Assert.Empty(porEstadoCerrado);
    }

    [Fact]
    public void AddHistory_AumentaElHistorial()
    {
        var ticket = NuevoTicket();
        _store.AddWithHistory(ticket, new TicketHistoryEntry { TicketId = ticket.Id, AtUtc = _ahora, Action = "Alta" });

        _store.AddHistory(new TicketHistoryEntry
        {
            TicketId = ticket.Id,
            AtUtc = _ahora.AddMinutes(10),
            Action = "Cambio de estado",
            FromState = TicketState.Nuevo,
            ToState = TicketState.EnAtencion
        });

        var historial = _store.GetHistory(ticket.Id);
        Assert.Equal(2, historial.Count);
        Assert.Equal(TicketState.EnAtencion, historial[1].ToState);
    }

    [Fact]
    public void Update_ReflejaLosCambios()
    {
        var ticket = NuevoTicket();
        _store.AddWithHistory(ticket, new TicketHistoryEntry { TicketId = ticket.Id, AtUtc = _ahora, Action = "Alta" });

        var editado = _store.Get(ticket.Id)!;
        editado.State = TicketState.Resuelto;
        editado.Resolution = "Se reemplazó el cable de poder y se validó el encendido.";
        editado.UpdatedUtc = _ahora.AddHours(1);
        _store.Update(editado);

        var recargado = _store.Get(ticket.Id)!;
        Assert.Equal(TicketState.Resuelto, recargado.State);
        Assert.Contains("cable", recargado.Resolution);
        Assert.Equal(1, _store.CountByState(TicketState.Resuelto));
    }
}
