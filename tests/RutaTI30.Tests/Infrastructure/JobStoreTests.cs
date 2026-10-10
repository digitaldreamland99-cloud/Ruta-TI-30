using RutaTI30.Domain;
using RutaTI30.Infrastructure;
using Xunit;

namespace RutaTI30.Tests.Infrastructure;

public class JobStoreTests : IDisposable
{
    private readonly TempDb _db = new();
    private readonly JobStore _store;

    public JobStoreTests()
    {
        _store = new JobStore(_db.Db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public void AddApplication_AsignaIdYSeRecupera()
    {
        var app = new JobApplication
        {
            Company = "Contoso SA",
            Role = "Soporte TI L1",
            Location = "Ciudad de México",
            DiscoveredUtc = DateTime.UtcNow,
            State = JobState.PorPostular,
            Url = "https://ejemplo.local/vacante"
        };

        var id = _store.AddApplication(app);
        var recargado = _store.GetApplication(id);

        Assert.True(id > 0);
        Assert.NotNull(recargado);
        Assert.Equal("Contoso SA", recargado!.Company);
        Assert.Equal(JobState.PorPostular, recargado.State);
    }

    [Fact]
    public void UpdateApplication_PersisteLosCambios()
    {
        var app = new JobApplication { Company = "Fabrikam", Role = "Mesa de Ayuda" };
        _store.AddApplication(app);

        app.State = JobState.Entrevista;
        app.InterviewUtc = new DateTime(2026, 2, 1, 15, 0, 0, DateTimeKind.Utc);
        app.Notes = "Entrevista telefónica programada.";
        _store.UpdateApplication(app);

        var recargado = _store.GetApplication(app.Id);
        Assert.Equal(JobState.Entrevista, recargado!.State);
        Assert.NotNull(recargado.InterviewUtc);
        Assert.Contains("telefónica", recargado.Notes);
    }

    [Fact]
    public void ListApplications_OrdenadoYCompleto()
    {
        _store.AddApplication(new JobApplication { Company = "A", Role = "Soporte" });
        _store.AddApplication(new JobApplication { Company = "B", Role = "Service Desk" });

        var lista = _store.ListApplications();

        Assert.Equal(2, lista.Count);
        Assert.Contains(lista, a => a.Company == "A");
        Assert.Contains(lista, a => a.Company == "B");
    }

    [Fact]
    public void FollowUps_SeAsocianASuPostulacion()
    {
        var app = new JobApplication { Company = "Northwind", Role = "L1" };
        _store.AddApplication(app);

        _store.AddFollowUp(new FollowUp { ApplicationId = app.Id, AtUtc = DateTime.UtcNow, Note = "Envié correo de seguimiento" });
        _store.AddFollowUp(new FollowUp { ApplicationId = app.Id, AtUtc = DateTime.UtcNow.AddHours(1), Note = "Sin respuesta; reprogramar" });

        var followUps = _store.GetFollowUps(app.Id);

        Assert.Equal(2, followUps.Count);
        Assert.All(followUps, f => Assert.Equal(app.Id, f.ApplicationId));
        Assert.Equal("Sin respuesta; reprogramar", followUps[0].Note); // más reciente primero
    }

    [Fact]
    public void InterviewAttempts_SoloIntentosDeLaPregunta()
    {
        _store.AddInterviewAttempt(new InterviewAttempt { QuestionId = "q01", AttemptedUtc = DateTime.UtcNow, AnswerText = "Respuesta 1", SelfRating = 3 });
        _store.AddInterviewAttempt(new InterviewAttempt { QuestionId = "q01", AttemptedUtc = DateTime.UtcNow.AddMinutes(5), AnswerText = "Respuesta 2", SelfRating = 4 });
        _store.AddInterviewAttempt(new InterviewAttempt { QuestionId = "q02", AttemptedUtc = DateTime.UtcNow, AnswerText = "Otra" });

        var deQ1 = _store.GetInterviewAttempts("q01");
        var todos = _store.GetInterviewAttempts();

        Assert.Equal(2, deQ1.Count);
        Assert.Equal(3, todos.Count);
        Assert.Equal(4, deQ1[0].SelfRating); // orden descendente: el más reciente primero
    }

    [Fact]
    public void DeleteApplication_EliminaLaPostulacionYSusSeguimientos()
    {
        var id = _store.AddApplication(new JobApplication { Company = "Borrar SA", Role = "L1" });
        _store.AddFollowUp(new FollowUp { ApplicationId = id, AtUtc = DateTime.UtcNow, Note = "Primer contacto" });
        _store.AddFollowUp(new FollowUp { ApplicationId = id, AtUtc = DateTime.UtcNow, Note = "Segundo contacto" });

        _store.DeleteApplication(id);

        Assert.Null(_store.GetApplication(id));
        Assert.Empty(_store.GetFollowUps(id));
        Assert.Empty(_store.ListApplications());
    }

    [Fact]
    public void DeleteApplication_NoAfectaA_OtrasPostulaciones()
    {
        var conservada = _store.AddApplication(new JobApplication { Company = "Conservar SA", Role = "L1" });
        var borrada = _store.AddApplication(new JobApplication { Company = "Temporal SA", Role = "L2" });
        _store.AddFollowUp(new FollowUp { ApplicationId = conservada, AtUtc = DateTime.UtcNow, Note = "Vigente" });

        _store.DeleteApplication(borrada);

        Assert.NotNull(_store.GetApplication(conservada));
        Assert.Single(_store.GetFollowUps(conservada));
        Assert.Null(_store.GetApplication(borrada));
    }

    [Fact]
    public void DeleteApplication_IdInexistente_NoLanzaError()
    {
        _store.DeleteApplication(9999);
        Assert.Empty(_store.ListApplications());
    }
}
