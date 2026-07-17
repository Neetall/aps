using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace ProductionScheduling.Test;

public class TabuSearchOptimizerTests
{
    private readonly ITestOutputHelper output;


    public TabuSearchOptimizerTests(
        ITestOutputHelper output)
    {
        this.output =
            output;
    }


    [Fact]
    public void TabuSearch_Should_Move_To_Better_Machine()
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
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                StartSlot =
                    0,

                DurationSlots =
                    2
            });


        timeline.Machines["M001"]
            .Occupy(
                0,
                2);


        var resourceIndex =
            TestAlgorithmFactory
                .CreateResourceIndex(
                    context);


        var ticketIndex =
            TestAlgorithmFactory
                .CreateJobTicketIndex(
                    context);


        /*
         * 注册Move
         */

        var moveSelector =
            new MoveSelector(
                new Random(1));


        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()),
            10);


        /*
         * 创建邻域生成器
         */

        var neighborhoodGenerator =
            new MoveNeighborhoodGenerator(
                moveSelector);


        var optimizer =
            new TabuSearchOptimizer(
                resourceIndex,
                ticketIndex,
                neighborhoodGenerator,
                new SolutionCloner(),
                new TabuSearchOptions
                {
                    Iterations =
                        50,

                    TabuTenure =
                        5,

                    AllowWorseMoves =
                        true
                });


        var evaluator =
            new ScheduleEvaluator();


        var before =
            evaluator.Evaluate(
                solution,
                timeline,
                context);


        output.WriteLine(
            $"Before Score:{before.Score}");


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
         * Output
         */

        TestSchedulePrinter.Print(
            output,
            "TabuSearch Result",
            result);


        TestScheduleSolutionPrinter.Print(
            output,
            result.Solution);


        /*
         * Assert
         */

        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timeline);


        Assert.NotNull(
            result.Evaluation);


        output.WriteLine(
            $"After Score:{result.Evaluation.Score}");


        Assert.True(
            result.Evaluation.Score <=
            before.Score);


        Assert.Equal(
            "M002",
            result.Solution
                .Operations[0]
                .MachineCode);


        Assert.Equal(
            1,
            result.Solution
                .Operations[0]
                .DurationSlots);


        /*
         * 验证原始解未污染
         */

        Assert.Equal(
            "M001",
            solution.Operations[0]
                .MachineCode);


        Assert.Equal(
            2,
            solution.Operations[0]
                .DurationSlots);
    }

    [Fact]
    public void TabuSearch_Should_Select_Best_Neighbor_From_Multiple_Moves()
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
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                StartSlot =
                    5,

                DurationSlots =
                    2
            });


        timeline.Machines["M001"]
            .Occupy(
                5,
                2);


        var resourceIndex =
            TestAlgorithmFactory
                .CreateResourceIndex(
                    context);


        var ticketIndex =
            TestAlgorithmFactory
                .CreateJobTicketIndex(
                    context);


        /*
         * 注册多个邻域
         */

        var moveSelector =
            new MoveSelector(
                new Random(1));


        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()),
            10);


        moveSelector.Register(
            new ShiftTimeMove(),
            10);


        var neighborhoodGenerator =
            new MoveNeighborhoodGenerator(
                moveSelector);


        var optimizer =
            new TabuSearchOptimizer(
                resourceIndex,
                ticketIndex,
                neighborhoodGenerator,
                new SolutionCloner(),
                new TabuSearchOptions
                {
                    Iterations =
                        50,

                    TabuTenure =
                        5,

                    AllowWorseMoves =
                        true
                });


        var evaluator =
            new ScheduleEvaluator();


        var before =
            evaluator.Evaluate(
                solution,
                timeline,
                context);


        output.WriteLine(
            $"Before:{before.Score}");


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
         * Output
         */

        TestSchedulePrinter.Print(
            output,
            "TabuSearch Multiple Neighbor Result",
            result);


        TestScheduleSolutionPrinter.Print(
            output,
            result.Solution);


        /*
         * Assert
         */

        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Evaluation);


        output.WriteLine(
            $"After:{result.Evaluation.Score}");


        /*
         * 必须找到更优邻域
         */

        Assert.True(
            result.Evaluation.Score <=
            before.Score);


        /*
         * 验证选择的是ChangeMachine产生的更优结果
         */

        Assert.Equal(
            "M002",
            result.Solution
                .Operations[0]
                .MachineCode);


        Assert.Equal(
            1,
            result.Solution
                .Operations[0]
                .DurationSlots);


        /*
         * 原解保持
         */

        Assert.Equal(
            "M001",
            solution.Operations[0]
                .MachineCode);


        Assert.Equal(
            5,
            solution.Operations[0]
                .StartSlot);
    }
}