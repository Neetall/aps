using System.Collections;

namespace ProductionScheduling.Timeline;

/// <summary>
/// 设备时间轴
/// 管理设备Slot占用状态
/// </summary>
public class MachineTimeline
{
    /// <summary>
    /// Slot占用状态
    /// true  = 已占用
    /// false = 空闲
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
            new BitArray(slotCount);
    }



    /// <summary>
    /// 设备编码
    /// </summary>
    public string MachineCode { get; }



    /// <summary>
    /// Slot数量
    /// </summary>
    public int SlotCount =>
        occupied.Length;



    /// <summary>
    /// 已占用Slot数量
    /// </summary>
    public int UsedSlotCount
    {
        get
        {
            var count = 0;

            for(var i = 0; i < occupied.Length; i++)
            {
                if(occupied[i])
                    count++;
            }

            return count;
        }
    }



    /// <summary>
    /// 空闲Slot数量
    /// </summary>
    public int FreeSlotCount =>
        SlotCount - UsedSlotCount;



    /// <summary>
    /// 是否全部空闲
    /// </summary>
    public bool IsEmpty =>
        UsedSlotCount == 0;



    /// <summary>
    /// 判断Slot是否空闲
    /// </summary>
    public bool IsFree(
        int slot)
    {
        ValidateSlot(slot);

        return !occupied[slot];
    }



    /// <summary>
    /// 判断是否可以占用连续Slot
    /// </summary>
    public bool CanOccupy(
        int startSlot,
        int duration)
    {
        if(startSlot < 0)
            return false;


        if(duration <= 0)
            return false;


        if(startSlot + duration > SlotCount)
            return false;



        for(
            var i = startSlot;
            i < startSlot + duration;
            i++)
        {
            if(occupied[i])
                return false;
        }


        return true;
    }



    /// <summary>
    /// 正常占用
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
    /// 强制占用
    /// 用于初始化维护、停机
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
    /// 释放占用
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
    /// 查找最早可用位置
    /// </summary>
    public int FindEarliest(
        int duration,
        int fromSlot = 0)
    {
        if(duration <= 0)
            return -1;


        if(fromSlot < 0)
            fromSlot = 0;


        for(
            var i = fromSlot;
            i <= SlotCount - duration;
            i++)
        {
            if(CanOccupy(
                   i,
                   duration))
            {
                return i;
            }
        }


        return -1;
    }



    /// <summary>
    /// 查找最晚可用位置
    /// </summary>
    public int FindLatest(
        int duration)
    {
        if(duration <= 0)
            return -1;


        for(
            var i = SlotCount - duration;
            i >= 0;
            i--)
        {
            if(CanOccupy(
                   i,
                   duration))
            {
                return i;
            }
        }


        return -1;
    }



    /// <summary>
    /// 获取连续空闲长度
    /// </summary>
    public int GetContinuousFreeLength(
        int startSlot)
    {
        ValidateSlot(startSlot);


        var length = 0;


        for(
            var i = startSlot;
            i < SlotCount;
            i++)
        {
            if(occupied[i])
                break;


            length++;
        }


        return length;
    }



    /// <summary>
    /// 获取所有占用Slot
    /// 调试使用
    /// </summary>
    public List<int> GetOccupiedSlots()
    {
        var result = new List<int>();

        for(var i = 0; i < SlotCount; i++)
        {
            if(occupied[i])
                result.Add(i);
        }

        return result;
    }



    /// <summary>
    /// 克隆
    /// GA/SA/LNS使用
    /// </summary>
    public MachineTimeline Clone()
    {
        var result =
            new MachineTimeline(
                MachineCode,
                SlotCount);


        result.occupied
            .Or(occupied);


        return result;
    }



    private void SetRange(
        int startSlot,
        int duration,
        bool value)
    {
        for(
            var i = startSlot;
            i < startSlot + duration;
            i++)
        {
            occupied[i] = value;
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


        if(startSlot + duration > SlotCount)
            throw new ArgumentException(
                "超过时间轴范围");
    }
}