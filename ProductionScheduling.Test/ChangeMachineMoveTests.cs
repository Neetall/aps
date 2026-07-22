using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class ChangeMachineMoveTests
{
    [Fact]
    public void ChangeMachineMove_Should_Move_To_Faster_Machine()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var factory =
            timelines.Factories["F001"];



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



        factory.Machines["M001"]
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
                    ticketIndex,

                CurrentOperation =
                    solution.Operations[0]
            };



        var move =
            new ChangeMachineMove(
                new ScheduleDurationCalculator(),
                TestAlgorithmFactory.CreateDebugOptions());



        var result =
            move.Apply(
                moveContext);



        Assert.True(
            result);



        var operation =
            solution.Operations[0];



        Assert.Equal(
            "M002",
            operation.MachineCode);



        Assert.Equal(
            1,
            operation.DurationSlots);



        Assert.False(
            factory.Machines["M002"]
                .IsFree(
                    operation.StartSlot));



        Assert.True(
            factory.Machines["M001"]
                .IsFree(
                    0));
    }
}
