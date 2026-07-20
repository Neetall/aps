using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestAlgorithmFactory
{
    public static IScheduler CreateGreedyScheduler(
        SchedulingContext context)
    {
        var resourceIndex =
            CreateResourceIndex(
                context);


        return new GreedyScheduler(
            new ScheduleDurationCalculator(),
            resourceIndex);
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


        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()),
            options.Moves.ChangeMachineWeight);


        moveSelector.Register(
            new ShiftTimeMove(),
            options.Moves.ShiftTimeWeight);


        moveSelector.Register(
            new SwapOperationMove(),
            options.Moves.SwapOperationWeight);



        return new LocalSearchOptimizer(
            resourceIndex,
            ticketIndex,
            new OperationSelector(
                new Random(1)),
            moveSelector,
            new SolutionCloner(),
            new SchedulingSolutionValidator(),
            options.LocalSearch);
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
                    options.Moves)
                .Create(),
            new SolutionCloner(),
            new AcceptanceCriteria(
                options.Acceptance),
            new SchedulingSolutionValidator(),
            options.SimulatedAnnealing);
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
        var resourceIndex =
            CreateResourceIndex(
                context);


        var ticketIndex =
            CreateJobTicketIndex(
                context);



        var moveSelectorFactory =
            new MoveSelectorFactory(
                options.Moves);



        var factoryContext =
            new OptimizerFactoryContext
            {
                ResourceIndex =
                    resourceIndex,

                JobTicketIndex =
                    ticketIndex,

                OperationSelector =
                    new OperationSelector(
                        new Random(1)),

                MoveSelectorFactory =
                    moveSelectorFactory,

                Cloner =
                    new SolutionCloner(),

                Validator =
                    new SchedulingSolutionValidator(),


                DestroyOperator =
                    destroyOperator,

                RepairOperator =
                    repairOperator,

                LnsAcceptance =
                    lnsAcceptance,

                NeighborhoodGenerator =
                    neighborhoodGenerator,

                Options =
                    options
            };



        var optimizerFactory =
            new OptimizerFactory(
                factoryContext);



        return new OptimizationPipelineRunner(
            options,
            optimizerFactory.Create);
    }
}