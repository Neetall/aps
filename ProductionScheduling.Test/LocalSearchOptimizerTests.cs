using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class LocalSearchOptimizerTests
{
    [Fact]
    public void LocalSearch_Should_Move_To_Better_Machine()
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
            TestSolutionFactory
                .CreateSlowMachineSolution(
                    timeline);


        var beforeMachine =
            solution.Operations[0]
                .MachineCode;


        var beforeDuration =
            solution.Operations[0]
                .DurationSlots;


        /*
         * 创建算法配置
         */

        var options =
            new SchedulingAlgorithmOptions
            {
                LocalSearch =
                    new LocalSearchOptions
                    {
                        Iterations = 20
                    }
            };


        /*
         * 创建优化器依赖
         */

        var resourceIndex =
            TestAlgorithmFactory
                .CreateResourceIndex(
                    context);


        var ticketIndex =
            TestAlgorithmFactory
                .CreateJobTicketIndex(
                    context);


        var moveSelector =
            new MoveSelectorFactory(
                    options.Moves)
                .Create();


        var optimizer =
            new LocalSearchOptimizer(
                resourceIndex,
                ticketIndex,
                new OperationSelector(
                    new Random(1)),
                moveSelector,
                new SolutionCloner(),
                options.LocalSearch);


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


        var after =
            result.Evaluation!;


        /*
         * Assert
         */

        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timeline);


        Assert.NotNull(
            result.Evaluation);


        /*
         * 应移动到快设备
         */

        Assert.Equal(
            "M002",
            result.Solution
                .Operations[0]
                .MachineCode);


        /*
         * 加工时间降低
         */

        Assert.Equal(
            1,
            result.Solution
                .Operations[0]
                .DurationSlots);


        /*
         * 评价改善
         */

        Assert.True(
            after.Score <
            before.Score);


        /*
         * 原始方案没有污染
         */

        Assert.Equal(
            beforeMachine,
            solution.Operations[0]
                .MachineCode);


        Assert.Equal(
            beforeDuration,
            solution.Operations[0]
                .DurationSlots);


        /*
         * 新时间轴占用正确
         */

        Assert.False(
            result.Timeline
                .Machines["M002"]
                .IsFree(
                    result.Solution
                        .Operations[0]
                        .StartSlot));


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