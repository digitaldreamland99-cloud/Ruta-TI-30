using RutaTI30.Domain;

namespace RutaTI30.Application.Services;

/// <summary>Resultado de la evaluación de un intento de laboratorio.</summary>
public sealed record LabScore(double Points, double MaxPoints, double Percentage, bool Passed,
    IReadOnlyList<string> UnsafeActions, IReadOnlyList<string> RecommendedMissed);

/// <summary>
/// Pondera las decisiones del usuario en un escenario. El resultado depende de las decisiones tomadas:
/// acciones recomendadas suman, acciones inseguras o prematuras restan, y no se premia una solución
/// rápida pero insegura.
/// </summary>
public static class LabScoring
{
    public const double PassPercentage = 70.0;

    public static LabScore Score(LabScenario scenario, IReadOnlyList<string> chosenActionIds)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        ArgumentNullException.ThrowIfNull(chosenActionIds);

        double points = 0;
        double max = scenario.Actions.Where(a => a.ScoreWeight > 0).Sum(a => (double)a.ScoreWeight);
        var unsafeActions = new List<string>();
        var missedRecommended = new List<string>();
        var chosen = chosenActionIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();

        foreach (var id in chosen)
        {
            var action = scenario.Actions.FirstOrDefault(a => a.Id == id);
            if (action is null) continue;
            points += action.ScoreWeight;
            if (action.ScoreWeight < 0) unsafeActions.Add(action.Label);
        }

        foreach (var recommended in scenario.Actions.Where(a => a.ScoreWeight > 0))
        {
            if (!chosen.Contains(recommended.Id))
                missedRecommended.Add(recommended.Label);
        }

        points = Math.Max(0, points);
        double percentage = max > 0 ? Math.Round(points / max * 100, 2) : 0;
        return new LabScore(points, max, percentage, percentage >= PassPercentage,
            unsafeActions, missedRecommended);
    }
}
