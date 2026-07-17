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
         * JT001 -> M001
         *
         * M001:
         * 50/h
         *
         * M002:
         * 100/h
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

                Timeline =
                    timeline,

                ResourceIndex =
                    resourceIndex,

                JobTicketIndex =
                    ticketIndex,

                CurrentOperation =
                    solution.Operations[0]
            };



        var move =
            new ChangeMachineMove(
                new ScheduleDurationCalculator());



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
            "M002",
            operation.MachineCode);



        Assert.Equal(
            1,
            operation.DurationSlots);



        Assert.False(
            timeline.Machines["M002"]
                .IsFree(
                    0));



        Assert.True(
            timeline.Machines["M001"]
                .IsFree(
                    0));
    }
}