using RutaTI30.Domain;

namespace RutaTI30.Application.Content;

/// <summary>Catálogo de lecciones de los 30 días (contenido educativo embebido, funciona sin Internet).</summary>
public interface ILessonCatalog
{
    IReadOnlyList<Lesson> Lessons { get; }
    Lesson? GetByDay(int day);
    Lesson? GetById(string id);
    IReadOnlyList<LearningDay> GetDays();
}

/// <summary>Implementación fija del catálogo: el contenido forma parte del binario.</summary>
public sealed class LessonCatalog : ILessonCatalog
{
    public static readonly LessonCatalog Instance = new(LessonsBloque1.All
        .Concat(LessonsBloque2.All)
        .Concat(LessonsBloque3.All)
        .Concat(LessonsBloque4.All)
        .Concat(LessonsBloque5.All)
        .OrderBy(l => l.Day)
        .ToList());

    private readonly IReadOnlyList<Lesson> _lessons;
    private readonly IReadOnlyList<LearningDay> _days;

    public LessonCatalog(IReadOnlyList<Lesson> lessons)
    {
        _lessons = lessons;
        _days = lessons.Select(l => new LearningDay
        {
            Day = l.Day,
            Block = l.Block,
            Title = l.Title,
            Objective = l.Objective,
            EstimatedMinutes = l.EstimatedMinutes,
            Topics = l.Topics.Count > 0 ? l.Topics : l.Questions.Select(q => q.Topic).Distinct().ToList()
        }).ToList();
    }

    public IReadOnlyList<Lesson> Lessons => _lessons;
    public IReadOnlyList<LearningDay> GetDays() => _days;
    public Lesson? GetByDay(int day) => _lessons.FirstOrDefault(l => l.Day == day);
    public Lesson? GetById(string id) => _lessons.FirstOrDefault(l => l.Id == id);
}
