namespace ProductionScheduling.Api.Domain.Request;

public class MachineCalendarBlockDto
{
    /// <summary>
    /// 不可用开始时间
    /// </summary>
    public DateTime StartTime { get; set; }


    /// <summary>
    /// 不可用结束时间
    /// </summary>
    public DateTime EndTime { get; set; }


    /// <summary>
    /// 类型
    /// </summary>
    public MachineCalendarBlockTypeDto Type { get; set; }


    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}