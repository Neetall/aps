namespace ProductionScheduling.Api.Domain.Request;

public class SchedulingOptionsDto
{
    /// <summary>
    /// 时间粒度
    /// 默认60分钟
    /// </summary>
    public int TimeGranularityMinutes { get; set; } = 60;


    /// <summary>
    /// 是否优化
    /// </summary>
    public bool EnableOptimization { get; set; } = true;


    /// <summary>
    /// 最大计算秒数
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}