using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestTimelineFactory
{
    public static TimelineContextGroup Create(
        SchedulingContext context)
    {
        var initializer =
            new TimelineInitializer();


        return initializer.Initialize(
            context);
    }



    public static TimelineContextGroup CreateEmpty()
    {
        var slots =
            new List<TimeSlot>();


        var start =
            DateTime.Today;


        for(var i = 0;
            i < 24;
            i++)
        {
            slots.Add(
                new TimeSlot
                {
                    StartTime =
                        start.AddHours(i),

                    EndTime =
                        start.AddHours(i + 1)
                });
        }



        var timeModel =
            new ContinuousTimeModel(
                slots);



        var factory =
            new FactoryTimeline(
                "F001",
                timeModel);



        factory.AddMachine(
            new MachineTimeline(
                "M001",
                timeModel.SlotCount));



        var group =
            new TimelineContextGroup();



        group.AddFactory(
            factory);



        return group;
    }
}