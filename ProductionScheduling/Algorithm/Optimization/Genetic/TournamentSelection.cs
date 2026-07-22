using ProductionScheduling.Algorithm.Configuration;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

public sealed class TournamentSelection
{
    private readonly GeneticAlgorithmOptions options;
    private readonly Random random;

    public TournamentSelection(
        GeneticAlgorithmOptions options)
        : this(
            options,
            Random.Shared)
    {
    }

    public TournamentSelection(
        GeneticAlgorithmOptions options,
        Random random)
    {
        this.options =
            options;

        this.random =
            random;
    }

    public GeneticIndividual Select(
        IReadOnlyList<GeneticIndividual> population)
    {
        if(population.Count == 0)
        {
            throw new ArgumentException(
                "种群不能为空",
                nameof(population));
        }

        GeneticIndividual? best =
            null;

        var tournamentSize =
            Math.Clamp(
                options.TournamentSize,
                1,
                population.Count);

        for(var index = 0;
            index < tournamentSize;
            index++)
        {
            var candidate =
                population[
                    random.Next(
                        population.Count)];

            if(best == null ||
               candidate.Fitness > best.Fitness)
            {
                best =
                    candidate;
            }
        }

        return best!;
    }
}
