using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Timeline;

public class TimelineBuilder
{
    public TimelineContextGroup Build(
        SchedulingContext context)
    {
        var timelines =
            new TimelineContextGroup();


        foreach(var factoryCalendar in context.FactoryCalendars)
        {
            var timeModel =
                BuildTimeModel(
                    factoryCalendar,
                    context.Options.TimeGranularityMinutes);



            var factoryTimeline =
                new FactoryTimeline(
                    factoryCalendar.FactoryCode,
                    timeModel);



            foreach(var machine in context.Machines
                        .Where(x =>
                            x.FactoryCode ==
                            factoryCalendar.FactoryCode))
            {
                factoryTimeline.AddMachine(
                    new MachineTimeline(
                        machine.Code,
                        timeModel.SlotCount));
            }



            timelines.AddFactory(
                factoryTimeline);
        }


        return timelines;
    }


    /// <summary>
    /// 创建工厂时间模型
    /// 每个工厂独立时间轴
    /// </summary>
    private ITimeModel BuildTimeModel(
        FactoryCalendar calendar,
        int granularity)
    {
        var slots =
            new List<TimeSlot>();


        foreach(var period in calendar.Periods)
        {
            AddPeriod(
                slots,
                period,
                granularity);
        }



        for(var i = 0;
            i < slots.Count;
            i++)
        {
            slots[i].Index =
                i;
        }



        return new ContinuousTimeModel(
            slots);
    }



    /// <summary>
    /// 根据班次生成Slot
    /// </summary>
    private void AddPeriod(
        List<TimeSlot> slots,
        ShiftPeriod period,
        int granularity)
    {
        var current =
            period.StartTime;



        while(current < period.EndTime)
        {
            var end =
                current.AddMinutes(
                    granularity);



            if(end > period.EndTime)
            {
                end =
                    period.EndTime;
            }



            slots.Add(
                new TimeSlot
                {
                    StartTime =
                        current,

                    EndTime =
                        end
                });



            current =
                end;
        }
    }
}