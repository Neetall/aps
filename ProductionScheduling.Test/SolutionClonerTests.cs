using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class SolutionClonerTests
{
    [Fact]
    public void Clone_Should_Create_Independent_State()
    {
        /*
         * Arrange
         */

        var source =
            new ScheduleState
            {
                Solution =
                    new SchedulingSolution
                    {
                        Operations =
                        [
                            new ScheduledOperation
                            {
                                JobTicketCode =
                                    "JT001",

                                MachineCode =
                                    "M001",

                                StartSlot =
                                    10,

                                DurationSlots =
                                    5
                            }
                        ]
                    },


                Timeline =
                    new TimelineContext(
                        new SchedulingTimeline()),


                Evaluation =
                    new EvaluationResult
                    {
                        Score = 100
                    },


                History =
                [
                    new MoveExecutionRecord
                    {
                        MoveName =
                            "ChangeMachine",

                        Success =
                            true,

                        JobTicketCode =
                            "JT001",

                        OldMachineCode =
                            "M001",

                        NewDurationSlots =
                            1
                    }
                ]
            };


        var cloner =
            new SolutionCloner();


        /*
         * Act
         */

        var clone =
            cloner.Clone(
                source);


        /*
         * 修改Clone
         */

        clone.Solution
            .Operations[0]
            .StartSlot = 100;


        clone.Evaluation!.Score = 50;


        clone.History[0]
                .MoveName =
            "ShiftTime";


        /*
         * Assert
         *
         * 原对象不能变化
         */


        Assert.Equal(
            10,
            source.Solution
                .Operations[0]
                .StartSlot);


        Assert.Equal(
            100,
            source.Evaluation!.Score);


        Assert.Equal(
            "ChangeMachine",
            source.History[0]
                .MoveName);
    }


    [Fact]
    public void Clone_Should_Copy_History()
    {
        var source =
            new ScheduleState
            {
                Solution =
                    new SchedulingSolution(),

                Timeline =
                    new TimelineContext(
                        new SchedulingTimeline()),


                History =
                [
                    new MoveExecutionRecord
                    {
                        MoveName =
                            "SwapOperation",

                        Success =
                            true
                    }
                ]
            };


        var clone =
            new SolutionCloner()
                .Clone(source);


        Assert.Single(
            clone.History);


        Assert.Equal(
            "SwapOperation",
            clone.History[0]
                .MoveName);


        Assert.NotSame(
            source.History,
            clone.History);
    }
}