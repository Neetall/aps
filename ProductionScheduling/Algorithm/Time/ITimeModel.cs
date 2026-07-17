using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Time;

public interface ITimeModel
{
    /// <summary>
    /// 全部Slot
    /// </summary>
    IReadOnlyList<TimeSlot> Slots { get; }

    /// <summary>
    /// Slot数量
    /// </summary>
    int SlotCount { get; }

    /// <summary>
    /// 获取指定Slot
    /// </summary>
    TimeSlot GetSlot(
        int slotIndex);

    /// <summary>
    /// 获取Slot开始时间
    /// </summary>
    DateTime GetSlotStart(
        int slotIndex);

    /// <summary>
    /// 获取Slot结束时间
    /// </summary>
    DateTime GetSlotEnd(
        int slotIndex);

    /// <summary>
    /// 根据时间查找Slot
    /// 不存在返回-1
    /// </summary>
    int GetSlotIndex(
        DateTime time);
    
    /// <summary>
    /// 是否存在该Slot
    /// </summary>
    bool ContainsSlot(
        int slotIndex);

    /// <summary>
    /// 是否属于有效生产时间
    /// </summary>
    bool IsWorkingSlot(
        int slotIndex);
}