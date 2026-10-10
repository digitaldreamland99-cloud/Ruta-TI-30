using RutaTI30.Application.Services;
using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Application;

public class ScheduleServiceTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly PreferenceStore _preferences;
    private readonly ScheduleService _service;

    public ScheduleServiceTests()
    {
        _preferences = new PreferenceStore(_db.Db);
        _service = new ScheduleService(_preferences);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public void HorarioPorDefecto_TieneLos11Bloques()
    {
        var schedule = _service.GetSchedule();
        Assert.Equal(11, schedule.Count);
    }

    [Fact]
    public void BlockAt_DetectaBloqueActivo()
    {
        var nueveTreinta = new DateTime(2026, 1, 5, 9, 30, 0);
        var bloque = _service.BlockAt(nueveTreinta);

        Assert.NotNull(bloque);
        Assert.Equal(ScheduleKind.Aprendizaje, bloque!.Kind);
    }

    [Fact]
    public void BlockAt_FueraDeHorario_DevuelveNull()
    {
        var ocho = new DateTime(2026, 1, 5, 8, 0, 0);
        Assert.Null(_service.BlockAt(ocho));
    }

    [Fact]
    public void BlockAt_EnElLimiteInclusivo()
    {
        var nueve = new DateTime(2026, 1, 5, 9, 0, 0);
        Assert.NotNull(_service.BlockAt(nueve));
    }

    [Fact]
    public void SaveSchedule_RechazaHorarioInvalido()
    {
        var invalido = new List<ScheduleBlock>
        {
            new() { Id = 1, StartMinutes = 600, EndMinutes = 540, Kind = ScheduleKind.Aprendizaje, Label = "Invertido" }
        };

        var errors = _service.SaveSchedule(invalido);

        Assert.NotEmpty(errors);
        // El horario guardado no cambió: sigue el default.
        Assert.Equal(11, _service.GetSchedule().Count);
    }

    [Fact]
    public void SaveSchedule_RechazaEtiquetaVacia()
    {
        var invalido = new List<ScheduleBlock>
        {
            new() { Id = 1, StartMinutes = 540, EndMinutes = 600, Kind = ScheduleKind.Aprendizaje, Label = "  " }
        };

        Assert.NotEmpty(_service.SaveSchedule(invalido));
    }

    [Fact]
    public void SaveSchedule_PersisteYRecupera()
    {
        var valido = new List<ScheduleBlock>
        {
            new() { Id = 1, StartMinutes = 8 * 60, EndMinutes = 9 * 60, Kind = ScheduleKind.Aprendizaje, Label = "Mañana" },
            new() { Id = 2, StartMinutes = 9 * 60, EndMinutes = 10 * 60, Kind = ScheduleKind.Repaso, Label = "Repaso" }
        };

        var errors = _service.SaveSchedule(valido);

        Assert.Empty(errors);
        var recargado = new ScheduleService(new PreferenceStore(_db.Db)).GetSchedule();
        Assert.Equal(2, recargado.Count);
        Assert.Equal("Mañana", recargado[0].Label);
    }
}
