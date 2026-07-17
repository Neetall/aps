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

                new TabuSearchOptions
                {
                    Iterations = 20,

                    TabuTenure = 5,

                    AllowWorseMoves = true
                });



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



        Assert.True(
            result.Evaluation.Score <=
            before.Score);



        /*
         * 原solution不能污染
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
}