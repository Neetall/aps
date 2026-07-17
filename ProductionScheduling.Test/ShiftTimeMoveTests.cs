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
         * JT001:
         *
         * M001
         *
         * Slot 0-1
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

                Timeline =
                    timeline,

                ResourceIndex =
                    resourceIndex,

                JobTicketIndex =
                    jobTicketIndex,

                CurrentOperation =
                    solution.Operations[0]
            };



        var move =
            new ShiftTimeMove();



        /*
         * Act
         */

        var result =
            move.Apply(
                moveContext);



        /*
         * Assert
         */

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



        Assert.False(
            timeline.Machines["M001"]
                .IsFree(
                    operation.StartSlot));



        Assert.True(
            timeline.Machines["M001"]
                .IsFree(
                    0));
    }
}