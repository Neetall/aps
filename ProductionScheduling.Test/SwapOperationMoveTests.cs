using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class SwapOperationMoveTests
{
    [Fact]
    public void SwapOperationMove_Should_Swap_Two_Operations()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSwapContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var solution =
            new SchedulingSolution();


        var op1 =
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
            };


        var op2 =
            new ScheduledOperation
            {
                FactoryCode =
                    "F001",

                JobTicketCode =
                    "JT002",

                MachineCode =
                    "M001",

                StartSlot =
                    2,

                DurationSlots =
                    2
            };


        solution.Operations.Add(
            op1);


        solution.Operations.Add(
            op2);



        var factoryTimeline =
            timelines.Factories["F001"];


        factoryTimeline.Machines["M001"]
            .Occupy(
                0,
                2);


        factoryTimeline.Machines["M001"]
            .Occupy(
                2,
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
                    op1
            };



        var move =
            new SwapOperationMove();



        var result =
            move.Apply(
                moveContext);



        Assert.True(
            result);



        var jt001 =
            solution.Operations[0];


        var jt002 =
            solution.Operations[1];



        Assert.Equal(
            "JT001",
            jt001.JobTicketCode);


        Assert.Equal(
            "JT002",
            jt002.JobTicketCode);



        Assert.Equal(
            2,
            jt001.StartSlot);


        Assert.Equal(
            0,
            jt002.StartSlot);



        Assert.Equal(
            2,
            jt001.DurationSlots);


        Assert.Equal(
            2,
            jt002.DurationSlots);



        Assert.False(
            factoryTimeline.Machines["M001"]
                .IsFree(
                    0));


        Assert.False(
            factoryTimeline.Machines["M001"]
                .IsFree(
                    2));
    }



    [Fact]
    public void SwapOperationMove_Should_Undo()
    {
  
    }
}