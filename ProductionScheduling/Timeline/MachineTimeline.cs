using System.Collections;

namespace ProductionScheduling.Timeline;

/// <summary>
///     设备资源时间占用
///
///     只负责:
///     1. 管理设备Slot占用状态
///     2. 判断资源是否可用
///     3. 修改占用状态
///
///     不负责:
///     1. 时间计算
///     2. Slot连续判断
///     3. 查找最早可用时间
/// </summary>
public class MachineTimeline
{
    /// <summary>
    ///     Slot占用状态
    ///
    ///     true  = 已占用
    ///     false = 空闲
    /// </summary>
    private readonly BitArray occupied;



    public MachineTimeline(
        string machineCode,
        int slotCount)
    {
        if(slotCount <= 0)
            throw new ArgumentException(
                "Slot数量必须大于0");


        MachineCode =
            machineCode;


        occupied =
            new BitArray(
                slotCount);
    }



    /// <summary>
    ///     设备编码
    /// </summary>
    public string MachineCode { get; }



    /// <summary>
    ///     Slot数量
    /// </summary>
    public int SlotCount =>
        occupied.Length;



    /// <summary>
    ///     已占用Slot数量
    /// </summary>
    public int UsedSlotCount
    {
        get
        {
            var count = 0;


            for(var i = 0;
                i < occupied.Length;
                i++)
            {
                if(occupied[i])
                    count++;
            }


            return count;
        }
    }



    /// <summary>
    ///     空闲Slot数量
    /// </summary>
    public int FreeSlotCount =>
        SlotCount -
        UsedSlotCount;



    /// <summary>
    ///     是否全部空闲
    /// </summary>
    public bool IsEmpty =>
        UsedSlotCount == 0;



    /// <summary>
    ///     判断Slot是否空闲
    /// </summary>
    public bool IsFree(
        int slot)
    {
        ValidateSlot(
            slot);


        return !occupied[slot];
    }



    /// <summary>
    ///     判断Slot是否占用
    /// </summary>
    public bool IsOccupied(
        int slot)
    {
        ValidateSlot(
            slot);


        return occupied[slot];
    }



    /// <summary>
    ///     判断区间是否可以占用
    ///
    ///     注意:
    ///     这里只判断资源占用
    ///     不判断时间连续性
    ///
    ///     时间连续性由 ITimeModel 负责
    /// </summary>
    public bool CanOccupy(
        int startSlot,
        int duration)
    {
        if(startSlot < 0)
            return false;


        if(duration <= 0)
            return false;


        if(startSlot + duration >
           SlotCount)
            return false;



        for(var i = startSlot;
            i < startSlot + duration;
            i++)
        {
            if(occupied[i])
                return false;
        }


        return true;
    }



    /// <summary>
    ///     正常占用
    /// </summary>
    public void Occupy(
        int startSlot,
        int duration)
    {
        if(!CanOccupy(
                startSlot,
                duration))
        {
            throw new InvalidOperationException(
                $"设备{MachineCode}时间段不可用");
        }


        SetRange(
            startSlot,
            duration,
            true);
    }



    /// <summary>
    ///     强制占用
    ///
    ///     用于:
    ///     1. 初始化设备停机
    ///     2. 维护时间
    /// </summary>
    public void ForceOccupy(
        int startSlot,
        int duration)
    {
        ValidateRange(
            startSlot,
            duration);


        SetRange(
            startSlot,
            duration,
            true);
    }



    /// <summary>
    ///     释放占用
    /// </summary>
    public void Release(
        int startSlot,
        int duration)
    {
        ValidateRange(
            startSlot,
            duration);


        SetRange(
            startSlot,
            duration,
            false);
    }



    /// <summary>
    ///     获取所有已占用Slot
    ///
    ///     调试、验证使用
    /// </summary>
    public List<int> GetOccupiedSlots()
    {
        var result =
            new List<int>();


        for(var i = 0;
            i < SlotCount;
            i++)
        {
            if(IsOccupied(i))
                result.Add(i);
        }


        return result;
    }



    /// <summary>
    ///     深复制
    ///
    ///     LNS/SA/Tabu使用
    /// </summary>
    public MachineTimeline Clone()
    {
        var result =
            new MachineTimeline(
                MachineCode,
                SlotCount);


        result.occupied
            .Or(
                occupied);


        return result;
    }



    private void SetRange(
        int startSlot,
        int duration,
        bool value)
    {
        for(var i = startSlot;
            i < startSlot + duration;
            i++)
        {
            occupied[i] =
                value;
        }
    }



    private void ValidateSlot(
        int slot)
    {
        if(slot < 0 ||
           slot >= SlotCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(slot));
        }
    }



    private void ValidateRange(
        int startSlot,
        int duration)
    {
        if(startSlot < 0)
            throw new ArgumentOutOfRangeException(
                nameof(startSlot));


        if(duration <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(duration));


        if(startSlot + duration >
           SlotCount)
        {
            throw new ArgumentException(
                "超过时间轴范围");
        }
    }
}