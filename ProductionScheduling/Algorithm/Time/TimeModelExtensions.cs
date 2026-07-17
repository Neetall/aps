using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Time;

public static class TimeModelExtensions
{
    /// <summary>
    /// 查找设备最早可用连续时间段
    ///
    /// 时间连续性由 ITimeModel 决定
    /// 资源占用由 MachineTimeline 决定
    /// </summary>
    public static int FindEarliestAvailable(
        this ITimeModel timeModel,
        MachineTimeline machine,
        int duration,
        int fromSlot = 0)
    {
        if(duration <= 0)
            return -1;


        if(fromSlot < 0)
            fromSlot = 0;



        for(var start = fromSlot;
            start <= timeModel.SlotCount - duration;
            start++)
        {
            if(CanOccupy(
                   timeModel,
                   machine,
                   start,
                   duration))
            {
                return start;
            }
        }


        return -1;
    }



    /// <summary>
    /// 判断一段时间是否可排
    /// </summary>
    public static bool CanOccupy(
        this ITimeModel timeModel,
        MachineTimeline machine,
        int startSlot,
        int duration)
    {
        if(duration <= 0)
            return false;


        if(!timeModel.ContainsSlot(
               startSlot))
            return false;



        var end =
            startSlot + duration;



        if(end > timeModel.SlotCount)
            return false;



        for(var i = startSlot;
            i < end;
            i++)
        {
            /*
             * 时间模型无效
             */
            if(!timeModel.IsWorkingSlot(i))
                return false;



            /*
             * 设备被占用
             */
            if(!machine.IsFree(i))
                return false;
        }


        return true;
    }
}