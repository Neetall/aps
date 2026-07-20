namespace ProductionScheduling.Api.Domain.Request;

public class ShiftPeriodDto
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }


    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
}