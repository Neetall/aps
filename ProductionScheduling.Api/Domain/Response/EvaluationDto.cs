namespace ProductionScheduling.Api.Domain.Response;

/// <summary>
/// 排产评价结果
///
/// API输出模型
/// </summary>
public class EvaluationDto
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
    /// 最大完工Slot数量
    /// </summary>
    public int MakespanSlots { get; set; }


    /// <summary>
    /// 总生产小时
    /// </summary>
    public double ProductionHours { get; set; }


    /// <summary>
    /// 设备利用率
    /// </summary>
    public double MachineUtilization { get; set; }


    /// <summary>
    /// 排程窗口内设备利用率
    /// </summary>
    public double ScheduleWindowMachineUtilization { get; set; }


    /// <summary>
    /// 瓶颈设备利用率
    /// </summary>
    public double BottleneckMachineUtilization { get; set; }


    /// <summary>
    /// 有排程任务的设备数量
    /// </summary>
    public int UsedMachineCount { get; set; }


    /// <summary>
    /// 总设备数量
    /// </summary>
    public int TotalMachineCount { get; set; }


    /// <summary>
    /// 延期订单数量
    /// </summary>
    public int DelayCount { get; set; }


    /// <summary>
    /// 延期惩罚
    /// </summary>
    public double DelayPenalty { get; set; }


    /// <summary>
    /// 延期说明
    /// </summary>
    public List<string> DelayMessages { get; set; } = [];
}
