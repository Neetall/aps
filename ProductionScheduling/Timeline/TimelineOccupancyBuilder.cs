using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Timeline;

/// <summary>
/// 设备占用时间构建器
///
/// 将设备不可用时间:
/// 维修
/// 停机
/// 已排产
///
/// 转换为MachineTimeline Slot占用
/// </summary>
public class TimelineOccupancyBuilder
{
    /// <summary>
    /// 是否忽略不存在的设备
    /// </summary>
    public bool IgnoreInvalidMachine { get; set; } = false;



    public void Build(
        TimelineContextGroup timelineContext,
        List<MachineCalendar> calendars)
    {
        foreach(var calendar in calendars)
        {
            if(!timelineContext.TryGetFactory(
                    calendar.FactoryCode,
                    out var factoryTimeline))
            {
                if(IgnoreInvalidMachine)
                    continue;


                throw new TimelineBuildException(
                    $"设备日历对应工厂不存在:" +
                    $"{calendar.FactoryCode}");
            }



            if(!factoryTimeline.TryGetMachine(
                    calendar.MachineCode,
                    out var machineTimeline))
            {
                if(IgnoreInvalidMachine)
                    continue;


                throw new TimelineBuildException(
                    $"设备日历对应设备不存在:" +
                    $"{calendar.FactoryCode}/" +
                    $"{calendar.MachineCode}");
            }



            foreach(var block in calendar.Blocks)
            {
                Occupy(
                    factoryTimeline.TimeModel,
                    machineTimeline,
                    block);
            }
        }
    }



    /// <summary>
    /// 将不可用时间转换为Slot占用
    /// </summary>
    private void Occupy(
        ITimeModel timeModel,
        MachineTimeline machineTimeline,
        MachineCalendarBlock block)
    {
        var startSlot =
            timeModel.GetSlotIndex(
                block.StartTime);



        var endSlot =
            timeModel.GetSlotIndex(
                block.EndTime);



        /*
         * 完全在时间轴范围之外
         */
        if(startSlot < 0 &&
           endSlot < 0)
        {
            return;
        }



        /*
         * 开始时间早于时间轴
         */
        if(startSlot < 0)
        {
            startSlot = 0;
        }



        /*
         * 结束时间晚于时间轴
         */
        if(endSlot < 0)
        {
            endSlot =
                timeModel.SlotCount;
        }



        var duration =
            endSlot -
            startSlot;



        if(duration <= 0)
            return;



        machineTimeline.ForceOccupy(
            startSlot,
            duration);
    }
}