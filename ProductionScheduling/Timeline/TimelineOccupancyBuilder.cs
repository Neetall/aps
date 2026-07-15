using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Timeline;

/// <summary>
///     设备占用时间构建器
///     将设备不可用时间转换为Slot占用
/// </summary>
public class TimelineOccupancyBuilder
{
    /// <summary>
    ///     是否忽略不存在的设备
    /// </summary>
    public bool IgnoreInvalidMachine { get; set; } = false;


    /// <summary>
    ///     构建设备占用
    /// </summary>
    public void Build(
        TimelineContext timelineContext,
        List<MachineCalendar> calendars)
    {
        foreach (var calendar in calendars)
        {
            if (!timelineContext.Machines
                    .TryGetValue(
                        calendar.MachineCode,
                        out var machineTimeline))
            {
                if (IgnoreInvalidMachine)
                    continue;


                throw new TimelineBuildException(
                    $"设备日历对应设备不存在:{calendar.MachineCode}");
            }


            foreach (var block in calendar.Blocks)
                Occupy(
                    timelineContext.Timeline,
                    machineTimeline,
                    block);
        }
    }


    /// <summary>
    ///     占用时间
    /// </summary>
    private void Occupy(
        SchedulingTimeline timeline,
        MachineTimeline machineTimeline,
        MachineCalendarBlock block)
    {
        var startSlot =
            timeline.GetStartSlot(
                block.StartTime);


        var endSlot =
            timeline.GetEndSlot(
                block.EndTime);


        /*
         * 说明：
         *
         * 完全不在排产范围
         *
         * 例如：
         *
         * 日历:
         * 8:00-18:00
         *
         * Block:
         * 20:00-22:00
         *
         */
        if (startSlot < 0 &&
            endSlot < 0)
            return;


        if (startSlot < 0) startSlot = 0;


        if (endSlot < 0)
            endSlot =
                timeline.Count;


        var duration =
            endSlot - startSlot;


        if (duration <= 0)
            return;


        machineTimeline.Occupy(
            startSlot,
            duration);
    }
}