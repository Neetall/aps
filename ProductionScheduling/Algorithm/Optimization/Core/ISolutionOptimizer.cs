using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Core;

/// <summary>
///     排产方案优化器
/// </summary>
public interface ISolutionOptimizer
{
    OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator);
}