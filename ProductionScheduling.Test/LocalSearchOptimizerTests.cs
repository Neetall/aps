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
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);


        var solution =
            TestSolutionFactory
                .CreateSlowMachineSolution(
                    timelines);


        var beforeMachine =
            solution.Operations[0]
                .MachineCode;


        var beforeDuration =
            solution.Operations[0]
                .DurationSlots;


        var options =
            new SchedulingAlgorithmOptions
            {
                LocalSearch =
                    new LocalSearchOptions
                    {
                        Iterations = 20
                    }
            };


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
                timelines,
                context);



        var result =
            optimizer.Optimize(
                solution,
                context,
                timelines,
                evaluator);



        var after =
            result.Evaluation!;


        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timelines);


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
            after.Score <
            before.Score);



        Assert.Equal(
            beforeMachine,
            solution.Operations[0]
                .MachineCode);



        Assert.Equal(
            beforeDuration,
            solution.Operations[0]
                .DurationSlots);



        var factoryTimeline =
            result.Timelines.Factories["F001"];


        Assert.False(
            factoryTimeline
                .Machines["M002"]
                .IsFree(
                    result.Solution
                        .Operations[0]
                        .StartSlot));



        Assert.True(
            factoryTimeline
                .Machines["M001"]
                .IsFree(
                    0));
    }
}