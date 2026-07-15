using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm;

/// <summary>
///     排产算法接口
/// </summary>
public interface IScheduler
{
    /// <summary>
    ///     执行排产
    /// </summary>
    /// <param name="context">
    ///     排产输入数据
    /// </param>
    /// <param name="timeline">
    ///     时间资源模型
    /// </param>
    /// <returns>
    ///     排产结果
    /// </returns>
    SchedulingSolution Schedule(
        SchedulingContext context,
        TimelineContext timeline);
}