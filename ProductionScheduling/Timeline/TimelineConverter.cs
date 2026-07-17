using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Timeline;

public class TimelineConverter
{
    public DateTime GetStartTime(
        SchedulingTimeline timeline,
        int slot)
    {
        Validate(
            timeline,
            slot);

        return timeline[slot]
            .StartTime;
    }


    public DateTime GetEndTime(
        SchedulingTimeline timeline,
        int slot)
    {
        Validate(
            timeline,
            slot);

        return timeline[slot]
            .EndTime;
    }


    public ShiftPeriod ToPeriod(
        SchedulingTimeline timeline,
        int startSlot,
        int duration)
    {
        return new ShiftPeriod
        {
            StartTime =
                timeline[startSlot]
                    .StartTime,

            EndTime =
                timeline[startSlot + duration - 1]
                    .EndTime
        };
    }


    private void Validate(
        SchedulingTimeline timeline,
        int slot)
    {
        if (slot < 0 ||
            slot >= timeline.Count)
            throw new ArgumentOutOfRangeException(
                nameof(slot));
    }
}