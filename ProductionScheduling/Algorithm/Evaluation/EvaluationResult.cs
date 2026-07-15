namespace ProductionScheduling.Algorithm.Evaluation;

/// <summary>
/// 排产方案评价结果
/// </summary>
public class EvaluationResult
{
    /// <summary>
    /// 总评分
    /// 越小越好
    /// </summary>
    public double Score { get; set; }



    /// <summary>
    /// 最大完工时间
    /// </summary>
    public DateTime EndTime { get; set; }



    /// <summary>
    /// 总生产小时
    /// </summary>
    public double ProductionHours { get; set; }



    /// <summary>
    /// 设备利用率
    /// </summary>
    public double MachineUtilization { get; set; }



    /// <summary>
    /// 延期数量
    /// </summary>
    public int DelayCount { get; set; }
}