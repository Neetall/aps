using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Timeline;

public class TimelineBuilder
{
    public TimelineContext Build(
        SchedulingContext context)
    {
        var timeModel =
            BuildTimeModel(
                context);



        var result =
            new TimelineContext(
                timeModel);



        foreach(var machine in context.Machines)
        {
            result.AddMachineTimeline(
                new MachineTimeline(
                    machine.Code,
                    timeModel.SlotCount));
        }



        ApplyMachineCalendars(
            result,
            context);



        return result;
    }



    private ITimeModel BuildTimeModel(
        SchedulingContext context)
    {
        var slots =
            new List<TimeSlot>();


        var granularity =
            context.Options
                .TimeGranularityMinutes;



        foreach(var calendar in context.FactoryCalendars)
        {
            foreach(var period in calendar.Periods)
            {
                AddPeriod(
                    slots,
                    period,
                    granularity);
            }
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
                end =
                    period.EndTime;



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



    private void ApplyMachineCalendars(
        TimelineContext timeline,
        SchedulingContext context)
    {
        foreach(var calendar in context.MachineCalendars)
        {
            if(!timeline.TryGetMachine(
                    calendar.MachineCode,
                    out var machine))
                continue;



            foreach(var block in calendar.Blocks)
            {
                ApplyBlock(
                    timeline.TimeModel,
                    machine,
                    block);
            }
        }
    }



    private void ApplyBlock(
        ITimeModel timeModel,
        MachineTimeline machine,
        MachineCalendarBlock block)
    {
        var start =
            timeModel.GetSlotIndex(
                block.StartTime);


        var end =
            timeModel.GetSlotIndex(
                block.EndTime);



        if(start < 0 &&
           end < 0)
            return;



        if(start < 0)
            start = 0;



        if(end < 0)
            end =
                timeModel.SlotCount;



        var duration =
            end - start;



        if(duration <= 0)
            return;



        machine.ForceOccupy(
            start,
            duration);
    }
}