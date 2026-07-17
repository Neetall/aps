using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class SolutionClonerTests
{
    private TimelineContextGroup CreateTimelines()
    {
        var slots =
            new List<TimeSlot>();


        for(var i = 0;
            i < 24;
            i++)
        {
            slots.Add(
                new TimeSlot
                {
                    StartTime =
                        DateTime.Today
                            .AddHours(i),

                    EndTime =
                        DateTime.Today
                            .AddHours(i + 1)
                });
        }


        var timeModel =
            new ContinuousTimeModel(
                slots);


        var timelines =
            new TimelineContextGroup();


        var factory =
            new FactoryTimeline(
                "F001",
                timeModel);


        factory.AddMachine(
            new MachineTimeline(
                "M001",
                slots.Count));


        timelines.AddFactory(
            factory);


        return timelines;
    }



    [Fact]
    public void Clone_Should_Create_Independent_State()
    {
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

                                FactoryCode =
                                    "F001",

                                StartSlot =
                                    10,

                                DurationSlots =
                                    5
                            }
                        ]
                    },

                

                Timelines =
                    CreateTimelines(),


                Evaluation =
                    new EvaluationResult
                    {
                        Score =
                            100
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



        var clone =
            cloner.Clone(
                source);



        clone.Solution
            .Operations[0]
            .StartSlot = 100;


        clone.Evaluation!.Score = 50;


        clone.History[0]
            .MoveName =
            "ShiftTime";



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


                Timelines =
                    CreateTimelines(),


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
                .Clone(
                    source);



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