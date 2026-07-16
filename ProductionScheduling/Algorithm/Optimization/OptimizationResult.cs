using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 优化结果
///
/// 包含排产方案和资源状态
/// </summary>
public class OptimizationResult
{
    /// <summary>
    /// 排产方案
    /// </summary>
    public SchedulingSolution Solution { get; set; } = null!;



    /// <summary>
    /// 时间资源状态
    /// </summary>
    public TimelineContext Timeline { get; set; } = null!;



    /// <summary>
    /// 评价结果
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }
}