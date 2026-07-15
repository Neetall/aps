using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 排产方案优化器
/// 
/// 输入一个可行方案
/// 输出优化后的方案
/// </summary>
public interface ISolutionOptimizer
{
    SchedulingSolution Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator);
}