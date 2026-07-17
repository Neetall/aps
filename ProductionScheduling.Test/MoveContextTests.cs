using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class MoveContextTests
{
    [Fact]
    public void MoveContext_Should_Hold_All_Move_Dependencies()
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


        var operation =
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
            };


        solution.Operations.Add(
            operation);


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
                    operation
            };



        Assert.Same(
            context,
            moveContext.SchedulingContext);


        Assert.Same(
            solution,
            moveContext.Solution);


        Assert.Same(
            timelines,
            moveContext.Timelines);


        Assert.Same(
            resourceIndex,
            moveContext.ResourceIndex);


        Assert.Same(
            ticketIndex,
            moveContext.JobTicketIndex);


        Assert.Same(
            operation,
            moveContext.CurrentOperation);
    }



    [Fact]
    public void MoveContext_Should_Allow_CurrentOperation_Change()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var solution =
            new SchedulingSolution();


        var operation1 =
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001"
            };


        var operation2 =
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT002"
            };


        solution.Operations.Add(
            operation1);


        solution.Operations.Add(
            operation2);



        var moveContext =
            new MoveContext
            {
                SchedulingContext =
                    context,

                Solution =
                    solution,

                CurrentOperation =
                    operation1
            };



        moveContext.CurrentOperation =
            operation2;



        Assert.Same(
            operation2,
            moveContext.CurrentOperation);
    }
}