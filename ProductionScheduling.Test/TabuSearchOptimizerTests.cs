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

namespace ProductionScheduling.Test;

public class TabuSearchOptimizerTests
{
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



        /*
         * 初始方案
         *
         * JT001 -> M001
         *
         * M001:
         * 100 / 50 = 2小时
         *
         * M002:
         * 100 / 100 = 1小时
         */

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



        var options =
            new TabuSearchOptions
            {
                Iterations = 50,

                TabuTenure = 5
            };



        var moveSelector =
            new MoveSelector(
                new Random(1));



        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()),
            10);



        var optimizer =
            new TabuSearchOptimizer(
                resourceIndex,

                ticketIndex,

                new OperationSelector(
                    new Random(1)),

                moveSelector,

                new SolutionCloner(),

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
            result.Evaluation);



        Assert.True(
            result.Evaluation.Score <=
            before.Score);



        var operation =
            result.Solution
                .Operations[0];



        Assert.Equal(
            "M002",
            operation.MachineCode);



        Assert.Equal(
            1,
            operation.DurationSlots);



        Assert.False(
            result.Timeline
                .Machines["M002"]
                .IsFree(
                    operation.StartSlot));



        Assert.True(
            result.Timeline
                .Machines["M001"]
                .IsFree(
                    0));
    }
}