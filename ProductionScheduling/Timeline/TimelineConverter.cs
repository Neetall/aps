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



    public List<ShiftPeriod> ToPeriods(
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



        var result =
            new List<ShiftPeriod>();


        var currentStartSlot =
            startSlot;


        var previousSlot =
            timeModel.GetSlot(
                startSlot);


        for(var slotIndex = startSlot + 1;
            slotIndex < startSlot + duration;
            slotIndex++)
        {
            var currentSlot =
                timeModel.GetSlot(
                    slotIndex);


            if(currentSlot.StartTime !=
               previousSlot.EndTime)
            {
                result.Add(
                    new ShiftPeriod
                    {
                        StartTime =
                            timeModel.GetSlotStart(
                                currentStartSlot),


                        EndTime =
                            previousSlot.EndTime
                    });


                currentStartSlot =
                    slotIndex;
            }


            previousSlot =
                currentSlot;
        }


        result.Add(
            new ShiftPeriod
            {
                StartTime =
                    timeModel.GetSlotStart(
                        currentStartSlot),


                EndTime =
                    timeModel.GetSlotEnd(
                        startSlot + duration - 1)
            });


        return result;
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
