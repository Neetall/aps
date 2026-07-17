using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;

namespace ProductionScheduling.Algorithm.Optimization.Tabu;

/// <summary>
/// Tabu Search邻域候选
/// </summary>
public class MoveCandidate
{
    /// <summary>
    /// 原始操作
    /// </summary>
    public ScheduledOperation Operation { get; set; } = null!;


    /// <summary>
    /// 使用的Move
    /// </summary>
    public IMove Move { get; set; } = null!;


    /// <summary>
    /// 执行后的状态
    /// </summary>
    public ScheduleState? State { get; set; }


    /// <summary>
    /// Move执行记录
    /// </summary>
    public MoveExecutionRecord? Record { get; set; }


    /// <summary>
    /// 评价结果
    /// </summary>
    public double Score { get; set; }
}