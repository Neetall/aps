using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class OptimizationPipelineTests
{
    [Fact]
    public void Pipeline_Should_Execute_Multiple_Optimizers()
    {
        /*
         * Arrange
         */

        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timeline =
            TestTimelineFactory
                .Create(
                    context);


        var solution =
            new SchedulingSolution
            {
                IsFeasible = true
            };


        var evaluator =
            new ScheduleEvaluator();



        var options =
            new SchedulingAlgorithmOptions();


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


        options.Pipeline.Add(
            new OptimizationStepOptions
            {
                Algorithm =
                    OptimizationAlgorithmType.SimulatedAnnealing,

                Enabled =
                    true,

                Order =
                    2
            });



        var executeOrder =
            new List<OptimizationAlgorithmType>();



        var runner =
            new OptimizationPipelineRunner(
                options,
                algorithm =>
                {
                    executeOrder.Add(
                        algorithm);


                    return new FakeOptimizer();
                });



        /*
         * Act
         */

        var result =
            runner.Run(
                solution,
                context,
                timeline,
                evaluator);



        /*
         * Assert
         */

        Assert.NotNull(
            result);


        Assert.Equal(
            2,
            executeOrder.Count);


        Assert.Equal(
            OptimizationAlgorithmType.LocalSearch,
            executeOrder[0]);


        Assert.Equal(
            OptimizationAlgorithmType.SimulatedAnnealing,
            executeOrder[1]);
    }



    private class FakeOptimizer : ISolutionOptimizer
    {
        public OptimizationResult Optimize(
            SchedulingSolution solution,
            SchedulingContext context,
            TimelineContext timeline,
            ScheduleEvaluator evaluator)
        {
            return new OptimizationResult
            {
                Solution =
                    solution,

                Timeline =
                    timeline,

                Evaluation =
                    evaluator.Evaluate(
                        solution,
                        timeline,
                        context)
            };
        }
    }
}