using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Timeline;

/// <summary>
/// 设备占用时间构建器
/// 将设备不可用时间转换为Slot占用
/// </summary>
public class TimelineOccupancyBuilder
{
    public bool IgnoreInvalidMachine { get; set; } = false;



    public void Build(
        TimelineContext timelineContext,
        List<MachineCalendar> calendars)
    {
        foreach(var calendar in calendars)
        {
            if(!timelineContext.Machines
                   .TryGetValue(
                       calendar.MachineCode,
                       out var machineTimeline))
            {
                if(IgnoreInvalidMachine)
                    continue;


                throw new TimelineBuildException(
                    $"设备日历对应设备不存在:{calendar.MachineCode}");
            }



            foreach(var block in calendar.Blocks)
            {
                Occupy(
                    timelineContext.TimeModel,
                    machineTimeline,
                    block);
            }
        }
    }



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
         * 完全不在排产范围
         */
        if(startSlot < 0 &&
           endSlot < 0)
        {
            return;
        }



        if(startSlot < 0)
            startSlot = 0;



        if(endSlot < 0)
            endSlot =
                timeModel.SlotCount;



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