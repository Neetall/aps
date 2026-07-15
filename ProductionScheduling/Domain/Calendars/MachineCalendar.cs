namespace ProductionScheduling.Domain.Calendars;

public class MachineCalendar
{
    /// <summary>
    ///     设备编码
    /// </summary>
    public string MachineCode { get; set; }

    /// <summary>
    ///     设备不可用时间段
    ///     包括：
    ///     1. 已排产任务占用
    ///     2. 设备维护
    ///     3. 临时停机
    /// </summary>
    public List<MachineCalendarBlock> Blocks { get; set; } = [];
}