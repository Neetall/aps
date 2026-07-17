using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.core;

public class LnsState
{
    /// <summary>
    /// 当前排产方案
    /// </summary>
    public SchedulingSolution Solution { get; set; } = null!;


    /// <summary>
    /// 多工厂时间资源状态
    /// 
    /// LNS过程中需要独立复制
    /// </summary>
    public TimelineContextGroup Timelines { get; set; } = null!;


    /// <summary>
    /// 当前评价结果
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }
}