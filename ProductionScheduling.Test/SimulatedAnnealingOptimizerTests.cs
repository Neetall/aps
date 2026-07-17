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



        var options =
            new SchedulingAlgorithmOptions
            {
                SimulatedAnnealing =
                    new SimulatedAnnealingOptions
                    {
                        Iterations =
                            100,

                        InitialTemperature =
                            100,

                        CoolingRate =
                            0.95
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
                timelines,
                context);



        var result =
            optimizer.Optimize(
                solution,
                context,
                timelines,
                evaluator);



        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timelines);


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



        Assert.Equal(
            "M002",
            operation.MachineCode);



        Assert.Equal(
            1,
            operation.DurationSlots);



        var factoryTimeline =
            result.Timelines
                .Factories["F001"];



        Assert.False(
            factoryTimeline
                .Machines["M002"]
                .IsFree(
                    operation.StartSlot));



        Assert.True(
            factoryTimeline
                .Machines["M001"]
                .IsFree(
                    0));
    }
}