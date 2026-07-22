namespace ProductionScheduling.Algorithm.Optimization.Genetic;

public sealed class OrderCrossover
{
    private readonly Random random;

    public OrderCrossover()
        : this(Random.Shared)
    {
    }

    public OrderCrossover(
        Random random)
    {
        this.random =
            random;
    }

    public GeneticIndividual Crossover(
        GeneticIndividual firstParent,
        GeneticIndividual secondParent,
        IReadOnlyList<string> jobTicketCodes)
    {
        var child =
            new GeneticIndividual();

        var firstOrder =
            CreateOrder(
                firstParent,
                jobTicketCodes);

        var secondOrder =
            CreateOrder(
                secondParent,
                jobTicketCodes);

        var childOrder =
            CreateOrderCrossover(
                firstOrder,
                secondOrder);

        for(var index = 0;
            index < childOrder.Count;
            index++)
        {
            child.PriorityGenes[
                childOrder[index]] =
                index;
        }

        foreach(var code in jobTicketCodes)
        {
            child.MachineGenes[code] =
                random.NextDouble() < 0.5 &&
                firstParent.MachineGenes.TryGetValue(
                    code,
                    out var firstMachine)
                    ? firstMachine
                    : secondParent.MachineGenes[
                        code];
        }

        child.Invalidate();

        return child;
    }

    private List<string> CreateOrder(
        GeneticIndividual individual,
        IReadOnlyList<string> jobTicketCodes)
    {
        return jobTicketCodes
            .OrderBy(code =>
                individual.PriorityGenes.TryGetValue(
                    code,
                    out var value)
                    ? value
                    : double.MaxValue)
            .ThenBy(
                code => code,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private List<string> CreateOrderCrossover(
        IReadOnlyList<string> firstOrder,
        IReadOnlyList<string> secondOrder)
    {
        if(firstOrder.Count <= 1)
            return firstOrder.ToList();

        var start =
            random.Next(
                firstOrder.Count);

        var end =
            random.Next(
                firstOrder.Count);

        if(start > end)
        {
            (start,end) =
                (end,start);
        }

        var child =
            Enumerable
                .Repeat<string?>(
                    null,
                    firstOrder.Count)
                .ToList();

        var used =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        for(var index = start;
            index <= end;
            index++)
        {
            child[index] =
                firstOrder[index];

            used.Add(
                firstOrder[index]);
        }

        var cursor =
            (end + 1) %
            firstOrder.Count;

        foreach(var code in secondOrder
                    .Skip(end + 1)
                    .Concat(secondOrder.Take(end + 1)))
        {
            if(!used.Add(code))
                continue;

            child[cursor] =
                code;

            cursor =
                (cursor + 1) %
                firstOrder.Count;
        }

        return child
            .Select(x => x!)
            .ToList();
    }
}
