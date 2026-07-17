using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestAlgorithmFactory
{
    /// <summary>
    /// 创建Greedy Scheduler
    /// </summary>
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



    /// <summary>
    /// 创建资源索引
    /// </summary>
    public static SchedulingResourceIndex CreateResourceIndex(
        SchedulingContext context)
    {
        var index =
            new SchedulingResourceIndex();


        index.Build(
            context.Machines);


        return index;
    }



    /// <summary>
    /// 创建工单索引
    /// </summary>
    public static JobTicketIndex CreateJobTicketIndex(
        SchedulingContext context)
    {
        var index =
            new JobTicketIndex();


        index.Build(
            context.Orders);


        return index;
    }



    /// <summary>
    /// 创建优化流水线
    /// </summary>
    public static OptimizationPipelineRunner CreatePipelineRunner(
        SchedulingContext context,
        SchedulingAlgorithmOptions options)
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



        var optimizerFactory =
            new OptimizerFactory(
                resourceIndex,
                ticketIndex,
                new OperationSelector(
                    new Random(1)),
                moveSelectorFactory,
                new SolutionCloner(),
                options);



        return new OptimizationPipelineRunner(
            options,
            optimizerFactory.Create);
    }
}