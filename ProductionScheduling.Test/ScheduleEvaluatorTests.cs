using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class ScheduleEvaluatorTests
{
    [Fact]
    public void Faster_Machine_Should_Have_Better_Score()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);


        var evaluator =
            new ScheduleEvaluator();



        var slowSolution =
            new SchedulingSolution();


        slowSolution.Operations.Add(
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


        timelines.Factories["F001"]
            .Machines["M001"]
            .Occupy(
                0,
                2);



        var slowResult =
            evaluator.Evaluate(
                slowSolution,
                timelines,
                context);



        var fastTimelines =
            TestTimelineFactory
                .Create(
                    context);



        var fastSolution =
            new SchedulingSolution();


        fastSolution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M002",

                StartSlot =
                    0,

                DurationSlots =
                    1
            });



        fastTimelines.Factories["F001"]
            .Machines["M002"]
            .Occupy(
                0,
                1);



        var fastResult =
            evaluator.Evaluate(
                fastSolution,
                fastTimelines,
                context);



        Assert.True(
            fastResult.Score <
            slowResult.Score);
    }
}