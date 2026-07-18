using Abituria.Models;

namespace Abituria.Services;

/// <summary>
/// Selects one exercise from a prepared pool without modifying the pool.
/// </summary>
public sealed class ExerciseRandomizer
{
    private readonly Random _random;

    public ExerciseRandomizer() : this(Random.Shared)
    {
    }

    public ExerciseRandomizer(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        _random = random;
    }

    public ExerciseDefinition? Select(IReadOnlyList<ExerciseDefinition> exercises)
    {
        ArgumentNullException.ThrowIfNull(exercises);
        return exercises.Count == 0 ? null : exercises[_random.Next(exercises.Count)];
    }
}
