using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

public sealed class GeneticAlgorithmOptimizer
    : ISolutionOptimizer
{
    private readonly GeneticAlgorithmOptions options;
    private readonly SchedulingResourceIndex resourceIndex;
    private readonly GeneticPopulationInitializer populationInitializer;
    private readonly GeneticDecoder decoder;
    private readonly TournamentSelection selection;
    private readonly OrderCrossover crossover;
    private readonly GeneticMutation mutation;
    private readonly SolutionCloner cloner;
    private readonly Random random;

    public GeneticAlgorithmOptimizer(
        SchedulingAlgorithmOptions options,
        SchedulingResourceIndex resourceIndex,
        GeneticPopulationInitializer populationInitializer,
        GeneticDecoder decoder,
        TournamentSelection selection,
        OrderCrossover crossover,
        GeneticMutation mutation,
        SolutionCloner cloner)
    {
        this.options =
            options.GeneticAlgorithm;

        this.resourceIndex =
            resourceIndex;

        this.populationInitializer =
            populationInitializer;

        this.decoder =
            decoder;

        this.selection =
            selection;

        this.crossover =
            crossover;

        this.mutation =
            mutation;

        this.cloner =
            cloner;

        random =
            Random.Shared;
    }

    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var evaluation =
            evaluator.Evaluate(
                solution,
                timelines,
                context);

        Console.WriteLine(
            $"GeneticAlgorithm开始 Score:{evaluation.Score}");

        var jobTicketCodes =
            context.Orders
                .SelectMany(x =>
                    x.JobTickets)
                .Select(x =>
                    x.Code)
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if(jobTicketCodes.Count == 0)
        {
            return new OptimizationResult
            {
                Solution = solution,
                Timelines = timelines,
                Evaluation = evaluation
            };
        }

        var candidateMachines =
            BuildCandidateMachines(
                jobTicketCodes);

        var population =
            populationInitializer.Initialize(
                solution,
                jobTicketCodes,
                candidateMachines,
                options.PopulationSize);

        EvaluatePopulation(
            population,
            context,
            evaluator);

        var bestIndividual =
            GetBest(
                population)
                .CloneEvaluated(
                    cloner);

        var noImprovementGenerations =
            0;

        for(var generation = 1;
            generation <= options.Generations;
            generation++)
        {
            var nextPopulation =
                CreateElitePopulation(
                    population);

            while(nextPopulation.Count < options.PopulationSize)
            {
                var firstParent =
                    selection.Select(
                        population);

                var secondParent =
                    selection.Select(
                        population);

                var child =
                    random.NextDouble() <= options.CrossoverRate
                        ? crossover.Crossover(
                            firstParent,
                            secondParent,
                            jobTicketCodes)
                        : firstParent.CloneGenes();

                mutation.Mutate(
                    child,
                    jobTicketCodes,
                    candidateMachines);

                decoder.Decode(
                    child,
                    context,
                    evaluator);

                nextPopulation.Add(
                    child);
            }

            population =
                nextPopulation;

            var generationBest =
                GetBest(
                    population);

            if(
                generationBest.Evaluation != null &&
                bestIndividual.Evaluation != null &&
                generationBest.Evaluation.Score <
                bestIndividual.Evaluation.Score)
            {
                bestIndividual =
                    generationBest.CloneEvaluated(
                        cloner);

                noImprovementGenerations =
                    0;

                Console.WriteLine(
                    $"GeneticAlgorithm第{generation}代改进 Score:{bestIndividual.Evaluation?.Score}");
            }
            else
            {
                noImprovementGenerations++;
            }

            if(noImprovementGenerations >=
               options.MaxNoImprovementGenerations)
            {
                Console.WriteLine(
                    $"GeneticAlgorithm提前结束，无改善代数:{noImprovementGenerations}");

                break;
            }
        }

        Console.WriteLine(
            $"GeneticAlgorithm结束 Score:{bestIndividual.Evaluation?.Score}");

        return new OptimizationResult
        {
            Solution = bestIndividual.Solution!,
            Timelines = bestIndividual.Timelines!,
            Evaluation = bestIndividual.Evaluation!
        };
    }

    private Dictionary<
        string,
        IReadOnlyList<string>> BuildCandidateMachines(
        IReadOnlyList<string> jobTicketCodes)
    {
        var result =
            new Dictionary<
                string,
                IReadOnlyList<string>>(
                StringComparer.OrdinalIgnoreCase);

        foreach(var code in jobTicketCodes)
        {
            result[code] =
                resourceIndex
                    .GetMachineCodes(
                        code)
                    .ToList();
        }

        return result;
    }

    private void EvaluatePopulation(
        IEnumerable<GeneticIndividual> population,
        SchedulingContext context,
        ScheduleEvaluator evaluator)
    {
        foreach(var individual in population)
        {
            decoder.Decode(
                individual,
                context,
                evaluator);
        }
    }

    private List<GeneticIndividual> CreateElitePopulation(
        IReadOnlyList<GeneticIndividual> population)
    {
        var eliteCount =
            Math.Clamp(
                options.EliteCount,
                0,
                options.PopulationSize);

        return population
            .OrderByDescending(x =>
                x.Fitness)
            .Take(
                eliteCount)
            .Select(x =>
                x.CloneEvaluated(
                    cloner))
            .ToList();
    }

    private GeneticIndividual GetBest(
        IEnumerable<GeneticIndividual> population)
    {
        return population
            .OrderByDescending(x =>
                x.Fitness)
            .First();
    }
}
