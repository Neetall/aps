using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 优化过程中的排产状态
/// </summary>
public class ScheduleState
{
    /// <summary>
    /// 排产方案
    /// </summary>
    public SchedulingSolution Solution { get; set; } = null!;


    /// <summary>
    /// 时间轴状态
    /// </summary>
    public TimelineContext Timeline { get; set; } = null!;


    /// <summary>
    /// 评价结果
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }

    /// <summary>
    /// 优化过程记录
    /// </summary>
    public List<MoveExecutionRecord> History { get; set; } = [];
    
    public void ApplyTo(
        SchedulingSolution solution,
        TimelineContext timeline)
    {
        solution =
            Solution;

        timeline =
            Timeline;
    }
}