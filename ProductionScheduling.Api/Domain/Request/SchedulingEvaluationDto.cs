namespace ProductionScheduling.Api.Domain.Request;

public class SchedulingEvaluationDto
{
    /// <summary>
    /// 综合评分
    /// </summary>
    public double Score { get; set; }


    /// <summary>
    /// 完工时间
    /// </summary>
    public DateTime? EndTime { get; set; }


    /// <summary>
    /// 最大完工Slot
    /// </summary>
    public int MakespanSlots { get; set; }


    /// <summary>
    /// 设备利用率
    /// </summary>
    public double MachineUtilization { get; set; }
}