using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Timeline;

/// <summary>
///     时间槽转换工具
/// </summary>
public class TimelineConverter
{
    private readonly SchedulingTimeline timeline;


    public TimelineConverter(
        SchedulingTimeline timeline)
    {
        this.timeline = timeline;
    }


    /// <summary>
    ///     Slot转换开始时间
    /// </summary>
    public DateTime GetStartTime(
        int slot)
    {
        Validate(slot);

        return timeline[slot]
            .StartTime;
    }


    /// <summary>
    ///     Slot转换结束时间
    /// </summary>
    public DateTime GetEndTime(
        int slot)
    {
        Validate(slot);

        return timeline[slot]
            .EndTime;
    }


    /// <summary>
    ///     Slot区间转换时间段
    /// </summary>
    public ShiftPeriod ToPeriod(
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
        int slot)
    {
        if (slot < 0 ||
            slot >= timeline.Count)
            throw new ArgumentOutOfRangeException(
                nameof(slot));
    }
}