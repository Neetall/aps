using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.CpSat;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Optimization.Genetic;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestAlgorithmFactory
{
    public static AlgorithmDebugOptions CreateDebugOptions()
    {
        return new AlgorithmDebugOptions();
    }



    public static IScheduler CreateGreedyScheduler(
        SchedulingContext context)
    {
        var resourceIndex =
            CreateResourceIndex(
                context);

        var durationCalculator =
            new ScheduleDurationCalculator();


        return new GreedyScheduler(
            resourceIndex,
            new SchedulePlacementService(
                durationCalculator,
                resourceIndex),
            CreateDebugOptions());
    }



    public static ISolutionOptimizer CreateLocalSearchOptimizer(
        SchedulingContext context,
        SchedulingAlgorithmOptions options)
    {
        var resourceIndex =
            CreateResourceIndex(
                context);


        var ticketIndex =
            CreateJobTicketIndex(
                context);


        var moveSelector =
            new MoveSelector(
                new Random(1));


        var durationCalculator =
            new ScheduleDurationCalculator();


        var debugOptions =
            CreateDebugOptions();


        moveSelector.Register(
            new ChangeMachineMove(
                durationCalculator,
                debugOptions),
            options.Moves.ChangeMachineWeight);


        moveSelector.Register(
            new ShiftTimeMove(
                debugOptions),
            options.Moves.ShiftTimeWeight);


        moveSelector.Register(
            new SwapOperationMove(
                debugOptions),
            options.Moves.SwapOperationWeight);



        return new LocalSearchOptimizer(
            resourceIndex,
            ticketIndex,
            new OperationSelector(
                new Random(1)),
            moveSelector,
            new SolutionCloner(),
            new SchedulingSolutionValidator(),
            options.LocalSearch,
            debugOptions);
    }



    public static SimulatedAnnealingOptimizer CreateSimulatedAnnealing(
        SchedulingContext context,
        SchedulingAlgorithmOptions options)
    {
        var resourceIndex =
            CreateResourceIndex(
                context);


        var ticketIndex =
            CreateJobTicketIndex(
                context);



        return new SimulatedAnnealingOptimizer(
            resourceIndex,
            ticketIndex,
            new OperationSelector(
                new Random(1)),
            new MoveSelectorFactory(
                    options.Moves,
                    new ScheduleDurationCalculator(),
                    CreateDebugOptions())
                .Create(),
            new SolutionCloner(),
            new AcceptanceCriteria(
                options.Acceptance),
            new SchedulingSolutionValidator(),
            options.SimulatedAnnealing,
            CreateDebugOptions());
    }



    public static SchedulingResourceIndex CreateResourceIndex(
        SchedulingContext context)
    {
        var index =
            new SchedulingResourceIndex();


        index.Build(
            context.Machines);


        return index;
    }



    public static JobTicketIndex CreateJobTicketIndex(
        SchedulingContext context)
    {
        var index =
            new JobTicketIndex();


        index.Build(
            context.Orders);


        return index;
    }



    public static OptimizationPipelineRunner CreatePipelineRunner(
        SchedulingContext context,
        SchedulingAlgorithmOptions options,
        IDestroyOperator destroyOperator,
        IRepairOperator repairOperator,
        ILnsAcceptance lnsAcceptance,
        MoveNeighborhoodGenerator neighborhoodGenerator)
    {
        var optimizerFactory =
            CreateOptimizerFactory(
                context,
                options,
                destroyOperator,
                repairOperator,
                lnsAcceptance,
                neighborhoodGenerator);



        return new OptimizationPipelineRunner(
            options,
            optimizerFactory);
    }



    public static OptimizerFactory CreateOptimizerFactory(
        SchedulingContext context,
        SchedulingAlgorithmOptions options,
        IDestroyOperator? destroyOperator = null,
        IRepairOperator? repairOperator = null,
        ILnsAcceptance? lnsAcceptance = null,
        MoveNeighborhoodGenerator? neighborhoodGenerator = null)
    {
        var resourceIndex =
            CreateResourceIndex(
                context);


        var ticketIndex =
            CreateJobTicketIndex(
                context);


        var durationCalculator =
            new ScheduleDurationCalculator();


        var debugOptions =
            CreateDebugOptions();


        var moveSelector =
            new MoveSelectorFactory(
                    options.Moves,
                    durationCalculator,
                    debugOptions)
                .Create();


        neighborhoodGenerator ??=
            new MoveNeighborhoodGenerator(
                moveSelector);


        destroyOperator ??=
            new RandomDestroyOperator();


        repairOperator ??=
            new GreedyRepairOperator(
                resourceIndex,
                ticketIndex,
                durationCalculator);


        lnsAcceptance ??=
            new LnsAcceptance();


        var cloner =
            new SolutionCloner();


        var validator =
            new SchedulingSolutionValidator();


        var placementService =
            new SchedulePlacementService(
                durationCalculator,
                resourceIndex);


        var timelineInitializer =
            new TimelineInitializer();


        return new OptimizerFactory(
            new LocalSearchOptimizer(
                resourceIndex,
                ticketIndex,
                new OperationSelector(
                    new Random(1)),
                moveSelector,
                cloner,
                validator,
                options.LocalSearch,
                debugOptions),
            new SimulatedAnnealingOptimizer(
                resourceIndex,
                ticketIndex,
                new OperationSelector(
                    new Random(1)),
                moveSelector,
                cloner,
                new AcceptanceCriteria(
                    options.Acceptance),
                validator,
                options.SimulatedAnnealing,
                debugOptions),
            new TabuSearchOptimizer(
                resourceIndex,
                ticketIndex,
                neighborhoodGenerator,
                cloner,
                validator,
                options.TabuSearch,
                debugOptions),
            new LnsOptimizer(
                cloner,
                destroyOperator,
                repairOperator,
                lnsAcceptance,
                validator,
                options.Lns,
                debugOptions),
            new GeneticAlgorithmOptimizer(
                options,
                resourceIndex,
                new GeneticPopulationInitializer(),
                new GeneticDecoder(
                    timelineInitializer,
                    placementService),
                new TournamentSelection(
                    options.GeneticAlgorithm),
                new OrderCrossover(),
                new GeneticMutation(
                    options.GeneticAlgorithm),
                cloner),
            new CpSatOptimizer(
                options,
                resourceIndex,
                durationCalculator,
                timelineInitializer,
                validator));
    }
}
