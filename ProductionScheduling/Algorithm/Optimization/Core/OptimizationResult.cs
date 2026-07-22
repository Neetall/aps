using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Core;

/// <summary>
///     优化结果
///     包含排产方案和资源状态
/// </summary>
public class OptimizationResult
{
    /// <summary>
    ///     排产方案
    /// </summary>
    public SchedulingSolution Solution { get; set; } = null!;


    /// <summary>
    ///     多工厂时间资源状态
    /// </summary>
    public TimelineContextGroup Timelines { get; set; } = null!;


    /// <summary>
    ///     评价结果
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }

    public bool TimedOut { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public double ElapsedMilliseconds { get; set; }

    public List<OptimizationAlgorithmResult> AlgorithmResults { get; set; } = [];
}
