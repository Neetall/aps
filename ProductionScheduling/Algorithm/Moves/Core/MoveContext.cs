using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Moves.Core;

/// <summary>
///     优化移动上下文
///     提供Move执行时需要的全部资源
/// </summary>
public class MoveContext
{
    /// <summary>
    ///     排产基础数据
    /// </summary>
    public SchedulingContext SchedulingContext { get; set; } = null!;


    /// <summary>
    ///     当前排产结果
    /// </summary>
    public SchedulingSolution Solution { get; set; } = null!;


    /// <summary>
    ///     时间资源
    /// </summary>
    public TimelineContext Timeline { get; set; } = null!;


    /// <summary>
    ///     设备能力索引
    /// </summary>
    public SchedulingResourceIndex ResourceIndex { get; set; } = null!;


    /// <summary>
    ///     当前优化目标
    /// </summary>
    public ScheduledOperation? CurrentOperation { get; set; }


    /// <summary>
    ///     派工单索引
    /// </summary>
    public JobTicketIndex JobTicketIndex { get; set; } = null!;


    /// <summary>
    ///     当前Move执行记录
    ///     用于:
    ///     1. Undo
    ///     2. SA接受/拒绝
    ///     3. Tabu记录
    /// </summary>
    public MoveExecutionRecord? ExecutionRecord { get; set; }
}