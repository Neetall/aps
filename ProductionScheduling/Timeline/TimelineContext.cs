namespace ProductionScheduling.Timeline;

/// <summary>
///     时间轴上下文
/// </summary>
public class TimelineContext
{
    /// <summary>
    ///     设备时间轴
    ///     Key = MachineCode
    /// </summary>
    private readonly Dictionary<string, MachineTimeline> machines = [];

    public TimelineContext(
        SchedulingTimeline timeline)
    {
        Timeline =
            timeline;
    }


    /// <summary>
    ///     全局时间轴
    /// </summary>
    public SchedulingTimeline Timeline { get; }


    /// <summary>
    ///     所有设备时间轴
    /// </summary>
    public IReadOnlyDictionary<string, MachineTimeline> Machines =>
        machines;


    /// <summary>
    ///     添加设备时间轴
    /// </summary>
    public void AddMachineTimeline(
        MachineTimeline timeline)
    {
        machines[timeline.MachineCode] =
            timeline;
    }


    /// <summary>
    ///     获取设备时间轴
    /// </summary>
    public bool TryGetMachine(
        string machineCode,
        out MachineTimeline timeline)
    {
        return machines.TryGetValue(
            machineCode,
            out timeline!);
    }
}