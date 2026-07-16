using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Timeline;

/// <summary>
///     时间轴构建器
///     根据排产上下文生成时间资源
/// </summary>
public class TimelineBuilder
{
    /// <summary>
    ///     构建时间上下文
    /// </summary>
    public TimelineContext Build(
        SchedulingContext context)
    {
        var timeline =
            BuildGlobalTimeline(
                context);


        var result =
            new TimelineContext(
                timeline);


        /*
         * 创建设备时间轴
         */
        foreach (var machine in context.Machines)
            result.AddMachineTimeline(
                new MachineTimeline(
                    machine.Code,
                    timeline.Count));


        /*
         * 初始化设备不可用时间
         */
        ApplyMachineCalendars(
            result,
            context);


        return result;
    }


    /// <summary>
    ///     构建全局时间轴
    /// </summary>
    private SchedulingTimeline BuildGlobalTimeline(
        SchedulingContext context)
    {
        var timeline =
            new SchedulingTimeline();


        var granularity =
            context.Options
                .TimeGranularityMinutes;


        foreach (var calendar in context.FactoryCalendars)
        foreach (var period in calendar.Periods)
            AddPeriod(
                timeline,
                period,
                granularity);


        return timeline;
    }


    /// <summary>
    ///     添加工作时间段
    /// </summary>
    private void AddPeriod(
        SchedulingTimeline timeline,
        ShiftPeriod period,
        int granularity)
    {
        var current =
            period.StartTime;


        while (current < period.EndTime)
        {
            var end =
                current.AddMinutes(
                    granularity);


            /*
             * 最后一段不足一个Slot
             */
            if (end > period.EndTime)
                end =
                    period.EndTime;


            timeline.AddSlot(
                new TimeSlot
                {
                    StartTime = current,
                    EndTime = end
                });


            current =
                end;
        }
    }


    /// <summary>
    ///     应用设备不可用时间
    /// </summary>
    private void ApplyMachineCalendars(
        TimelineContext timelineContext,
        SchedulingContext context)
    {
        foreach (var calendar in context.MachineCalendars)
        {
            if (!timelineContext.TryGetMachine(
                    calendar.MachineCode,
                    out var machineTimeline))
                continue;


            foreach (var block in calendar.Blocks)
                ApplyBlock(
                    timelineContext.Timeline,
                    machineTimeline,
                    block);
        }
    }


    /// <summary>
    ///     应用一个设备占用块
    /// </summary>
    private void ApplyBlock(
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
         * 时间不在排产范围内
         */
        if (startSlot < 0 ||
            endSlot < 0)
            return;


        var duration =
            endSlot - startSlot;


        if (duration <= 0)
            return;


        machineTimeline.ForceOccupy(
            startSlot,
            duration);
    }
}