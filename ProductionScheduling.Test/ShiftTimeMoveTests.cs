using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class ShiftTimeMoveTests
{
    [Fact]
    public void ShiftTimeMove_Should_Move_Operation_To_New_Time()
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
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                FactoryCode =
                    "F001",

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



        var resourceIndex =
            TestAlgorithmFactory
                .CreateResourceIndex(
                    context);


        var jobTicketIndex =
            TestAlgorithmFactory
                .CreateJobTicketIndex(
                    context);



        var moveContext =
            new MoveContext
            {
                SchedulingContext =
                    context,

                Solution =
                    solution,

                Timelines =
                    timelines,

                ResourceIndex =
                    resourceIndex,

                JobTicketIndex =
                    jobTicketIndex,

                CurrentOperation =
                    solution.Operations[0]
            };



        var move =
            new ShiftTimeMove(
                TestAlgorithmFactory.CreateDebugOptions());



        var result =
            move.Apply(
                moveContext);



        Assert.True(
            result);



        var operation =
            solution.Operations[0];



        Assert.Equal(
            "JT001",
            operation.JobTicketCode);


        Assert.Equal(
            "M001",
            operation.MachineCode);


        Assert.NotEqual(
            0,
            operation.StartSlot);



        var machine =
            timelines.Factories["F001"]
                .Machines["M001"];



        Assert.False(
            machine.IsFree(
                operation.StartSlot));



        Assert.True(
            machine.IsFree(
                0));
    }
}
