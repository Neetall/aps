namespace ProductionScheduling.Domain.Calendars;

public class MachineCalendarBlock
{
    /// <summary>
    ///     不可用开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    ///     不可用结束时间
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    ///     类型
    /// </summary>
    public MachineBlockType Type { get; set; } = MachineBlockType.Scheduled;

    /// <summary>
    ///     备注
    /// </summary>
    public string? Remark { get; set; }
}

public enum MachineBlockType
{
    /// <summary>
    ///     已排产占用
    /// </summary>
    Scheduled = 1,

    /// <summary>
    ///     设备维修
    /// </summary>
    Maintenance = 2,

    /// <summary>
    ///     临时停机
    /// </summary>
    Downtime = 3
}