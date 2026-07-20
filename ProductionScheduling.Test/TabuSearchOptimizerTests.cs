using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
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
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                FactoryCode =
                    "F001",

                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                StartSlot =
                    0,

                DurationSlots =
                    2
            });



        var factoryTimeline =
            timelines.Factories["F001"];


        factoryTimeline.Machines["M001"]
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



        var neighborhoodGenerator =
            new MoveNeighborhoodGenerator(
                moveSelector);



        var optimizer =
            new TabuSearchOptimizer(
                resourceIndex,
                ticketIndex,
                neighborhoodGenerator,
                new SolutionCloner(),
                new SchedulingSolutionValidator(),
                new TabuSearchOptions
                {
                    Iterations = 50,

                    TabuTenure = 5,

                    AllowWorseMoves = true
                });



        var evaluator =
            new ScheduleEvaluator();



        var before =
            evaluator.Evaluate(
                solution,
                timelines,
                context);



        var result =
            optimizer.Optimize(
                solution,
                context,
                timelines,
                evaluator);



        TestSchedulePrinter.Print(
            output,
            "TabuSearch Result",
            result);


        TestScheduleSolutionPrinter.Print(
            output,
            result.Solution);



        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timelines);


        Assert.NotNull(
            result.Evaluation);



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
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                FactoryCode =
                    "F001",

                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                StartSlot =
                    5,

                DurationSlots =
                    2
            });



        var factoryTimeline =
            timelines.Factories["F001"];


        factoryTimeline.Machines["M001"]
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
                new SchedulingSolutionValidator(),
                new TabuSearchOptions
                {
                    Iterations = 50,

                    TabuTenure = 5,

                    AllowWorseMoves = true
                });



        var evaluator =
            new ScheduleEvaluator();



        var before =
            evaluator.Evaluate(
                solution,
                timelines,
                context);



        var result =
            optimizer.Optimize(
                solution,
                context,
                timelines,
                evaluator);



        TestSchedulePrinter.Print(
            output,
            "TabuSearch Multiple Neighbor Result",
            result);


        TestScheduleSolutionPrinter.Print(
            output,
            result.Solution);



        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Evaluation);



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