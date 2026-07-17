using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class SimulatedAnnealingOptimizerTests
{
    [Fact]
    public void SimulatedAnnealing_Should_Return_Better_Or_Equal_Solution()
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


        /*
         * 创建较差初始方案
         *
         * JT001 -> M001
         *
         * M001:
         * 50/h
         *
         * M002:
         * 100/h
         */
        var solution =
            TestSolutionFactory
                .CreateSlowMachineSolution(
                    timeline);


        var options =
            new SchedulingAlgorithmOptions
            {
                SimulatedAnnealing =
                    new SimulatedAnnealingOptions
                    {
                        Iterations = 100,

                        InitialTemperature = 100,

                        CoolingRate = 0.95
                    }
            };


        var optimizer =
            TestAlgorithmFactory
                .CreateSimulatedAnnealing(
                    context,
                    options);


        var evaluator =
            new ScheduleEvaluator();


        var before =
            evaluator.Evaluate(
                solution,
                timeline,
                context);


        /*
         * Act
         */

        var result =
            optimizer.Optimize(
                solution,
                context,
                timeline,
                evaluator);


        /*
         * Assert
         */

        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timeline);


        Assert.NotNull(
            result.Evaluation);


        Assert.True(
            result.Solution.IsFeasible);


        Assert.True(
            result.Evaluation.Score <=
            before.Score);


        var operation =
            result.Solution
                .Operations[0];


        /*
         * 应迁移到快设备
         */
        Assert.Equal(
            "M002",
            operation.MachineCode);


        /*
         * 加工时间降低
         */
        Assert.Equal(
            1,
            operation.DurationSlots);


        /*
         * 新设备占用
         */
        Assert.False(
            result.Timeline
                .Machines["M002"]
                .IsFree(
                    operation.StartSlot));


        /*
         * 原设备释放
         */
        Assert.True(
            result.Timeline
                .Machines["M001"]
                .IsFree(
                    0));
    }
}