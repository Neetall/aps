namespace ProductionScheduling.Timeline;

/// <summary>
///     调度时间槽
///     一个TimeSlot代表一个最小调度单位
/// </summary>
public class TimeSlot
{
    /// <summary>
    ///     全局时间槽索引
    ///     所有设备共享
    /// </summary>
    public int Index { get; internal set; }


    /// <summary>
    ///     开始时间
    /// </summary>
    public DateTime StartTime { get; set; }


    /// <summary>
    ///     结束时间
    /// </summary>
    public DateTime EndTime { get; set; }


    /// <summary>
    ///     时间长度
    /// </summary>
    public TimeSpan Duration =>
        EndTime - StartTime;


    /// <summary>
    ///     判断时间是否属于当前Slot
    /// </summary>
    public bool Contains(
        DateTime time)
    {
        return time >= StartTime &&
               time < EndTime;
    }
}