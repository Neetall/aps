using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Timeline;

public class TimelineConverter
{
    public DateTime GetStartTime(
        ITimeModel timeModel,
        int slot)
    {
        Validate(
            timeModel,
            slot);


        return timeModel.GetSlotStart(
            slot);
    }



    public DateTime GetEndTime(
        ITimeModel timeModel,
        int slot)
    {
        Validate(
            timeModel,
            slot);


        return timeModel.GetSlotEnd(
            slot);
    }



    public ShiftPeriod ToPeriod(
        ITimeModel timeModel,
        int startSlot,
        int duration)
    {
        if(duration <= 0)
            throw new ArgumentException(
                "Duration必须大于0");



        Validate(
            timeModel,
            startSlot);



        Validate(
            timeModel,
            startSlot + duration - 1);



        return new ShiftPeriod
        {
            StartTime =
                timeModel.GetSlotStart(
                    startSlot),


            EndTime =
                timeModel.GetSlotEnd(
                    startSlot + duration - 1)
        };
    }



    private void Validate(
        ITimeModel timeModel,
        int slot)
    {
        if(!timeModel.ContainsSlot(
               slot))
        {
            throw new ArgumentOutOfRangeException(
                nameof(slot));
        }
    }
}