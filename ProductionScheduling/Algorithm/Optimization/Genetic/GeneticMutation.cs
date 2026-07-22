using ProductionScheduling.Algorithm.Configuration;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

public sealed class GeneticMutation
{
    private readonly GeneticAlgorithmOptions options;
    private readonly Random random;

    public GeneticMutation(
        GeneticAlgorithmOptions options)
        : this(
            options,
            Random.Shared)
    {
    }

    public GeneticMutation(
        GeneticAlgorithmOptions options,
        Random random)
    {
        this.options =
            options;

        this.random =
            random;
    }

    public void Mutate(
        GeneticIndividual individual,
        IReadOnlyList<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines)
    {
        if(random.NextDouble() >
           options.MutationRate)
        {
            return;
        }

        MutatePriority(
            individual,
            jobTicketCodes);

        MutateMachine(
            individual,
            jobTicketCodes,
            candidateMachines);

        individual.Invalidate();
    }

    private void MutatePriority(
        GeneticIndividual individual,
        IReadOnlyList<string> jobTicketCodes)
    {
        if(jobTicketCodes.Count < 2)
            return;

        var firstIndex =
            random.Next(
                jobTicketCodes.Count);

        var secondIndex =
            random.Next(
                jobTicketCodes.Count - 1);

        if(secondIndex >= firstIndex)
            secondIndex++;

        var firstCode =
            jobTicketCodes[firstIndex];

        var secondCode =
            jobTicketCodes[secondIndex];

        (
            individual.PriorityGenes[firstCode],
            individual.PriorityGenes[secondCode]
        )
        =
        (
            individual.PriorityGenes[secondCode],
            individual.PriorityGenes[firstCode]
        );
    }

    private void MutateMachine(
        GeneticIndividual individual,
        IReadOnlyList<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines)
    {
        if(jobTicketCodes.Count == 0)
            return;

        var code =
            jobTicketCodes[
                random.Next(
                    jobTicketCodes.Count)];

        var machines =
            candidateMachines[code]
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if(machines.Count == 0)
            return;

        var current =
            individual.MachineGenes.TryGetValue(
                code,
                out var machineCode)
                ? machineCode
                : null;

        var alternatives =
            machines
                .Where(x =>
                    !string.Equals(
                        x,
                        current,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

        var source =
            alternatives.Count > 0
                ? alternatives
                : machines;

        individual.MachineGenes[code] =
            source[
                random.Next(
                    source.Count)];
    }
}
