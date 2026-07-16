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
    /// 创建默认算法配置
    /// </summary>
    public static SchedulingAlgorithmOptions CreateOptions()
    {
        var options =
            new SchedulingAlgorithmOptions
            {
                LocalSearch =
                    new LocalSearchOptions
                    {
                        Iterations = 50
                    },
                SimulatedAnnealing =
                    new SimulatedAnnealingOptions
                    {
                        Iterations = 100
                    }
                
            };


        options.Pipeline.Add(
            new OptimizationStepOptions
            {
                Algorithm =
                    OptimizationAlgorithmType.LocalSearch,

                Enabled =
                    true,

                Order =
                    1
            });


        return options;
    }



    /// <summary>
    /// 创建优化流水线
    /// </summary>
    public static OptimizationPipelineRunner CreatePipelineRunner(
        SchedulingContext context)
    {
        var options =
            CreateOptions();



        var resourceIndex =
            new SchedulingResourceIndex();


        resourceIndex.Build(
            context.Machines);



        var ticketIndex =
            new JobTicketIndex();


        ticketIndex.Build(
            context.Orders);



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