namespace ProductionScheduling.Timeline;

/// <summary>
///     全局排产时间轴
///     管理DateTime和Slot映射
/// </summary>
public class SchedulingTimeline
{
    private readonly Dictionary<DateTime, int> endIndexMap = new();


    private readonly List<TimeSlot> slots = [];
    private readonly Dictionary<DateTime, int> startIndexMap = new();


    /// <summary>
    ///     所有Slot
    /// </summary>
    public IReadOnlyList<TimeSlot> Slots =>
        slots;


    /// <summary>
    ///     Slot数量
    /// </summary>
    public int Count =>
        slots.Count;


    public TimeSlot this[int index]
        => slots[index];


    /// <summary>
    ///     添加Slot
    /// </summary>
    public void AddSlot(
        TimeSlot slot)
    {
        if (slot.StartTime >= slot.EndTime)
            throw new ArgumentException(
                "Slot时间错误");


        slot.Index =
            slots.Count;


        slots.Add(slot);


        startIndexMap
                [slot.StartTime]
            =
            slot.Index;


        endIndexMap
                [slot.EndTime]
            =
            slot.Index + 1;
    }


    /// <summary>
    ///     开始时间转Slot
    /// </summary>
    public int GetStartSlot(
        DateTime time)
    {
        if (startIndexMap
            .TryGetValue(
                time,
                out var index))
            return index;


        return FindSlot(time);
    }


    /// <summary>
    ///     结束时间转Slot
    /// </summary>
    public int GetEndSlot(
        DateTime time)
    {
        if (endIndexMap
            .TryGetValue(
                time,
                out var index))
            return index;


        return FindSlot(time);
    }


    /// <summary>
    ///     Slot转开始时间
    /// </summary>
    public DateTime GetStartTime(
        int slot)
    {
        ValidateSlot(slot);

        return slots[slot]
            .StartTime;
    }


    /// <summary>
    ///     Slot转结束时间
    /// </summary>
    public DateTime GetEndTime(
        int slot)
    {
        ValidateSlot(slot);

        return slots[slot]
            .EndTime;
    }


    /// <summary>
    ///     Slot范围结束时间
    /// </summary>
    public DateTime GetEndTime(
        int startSlot,
        int duration)
    {
        ValidateSlot(startSlot);

        var end =
            startSlot + duration - 1;


        ValidateSlot(end);


        return slots[end]
            .EndTime;
    }


    /// <summary>
    ///     二分查找Slot
    /// </summary>
    private int FindSlot(
        DateTime time)
    {
        var left = 0;

        var right =
            slots.Count - 1;


        while (left <= right)
        {
            var mid =
                left +
                (right - left) / 2;


            var slot =
                slots[mid];


            if (time < slot.StartTime)
                right =
                    mid - 1;
            else if (time >= slot.EndTime)
                left =
                    mid + 1;
            else
                return mid;
        }


        return -1;
    }


    private void ValidateSlot(
        int slot)
    {
        if (slot < 0 ||
            slot >= Count)
            throw new ArgumentOutOfRangeException(
                nameof(slot));
    }
}