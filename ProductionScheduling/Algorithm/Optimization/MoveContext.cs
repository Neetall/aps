using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 优化移动上下文
///
/// 提供Move执行时需要的全部资源
/// </summary>
public class MoveContext
{
    /// <summary>
    /// 排产基础数据
    /// </summary>
    public SchedulingContext SchedulingContext { get; set; }



    /// <summary>
    /// 当前排产结果
    /// </summary>
    public SchedulingSolution Solution { get; set; }



    /// <summary>
    /// 时间资源
    /// </summary>
    public TimelineContext Timeline { get; set; }



    /// <summary>
    /// 设备能力索引
    /// </summary>
    public SchedulingResourceIndex ResourceIndex { get; set; }
    
    /// <summary>
    /// 当前优化目标
    /// </summary>
    public ScheduledOperation? CurrentOperation { get; set; }
    
    /// <summary>
    /// 派工单索引
    /// </summary>
    public JobTicketIndex JobTicketIndex { get; set; } = null!;
}