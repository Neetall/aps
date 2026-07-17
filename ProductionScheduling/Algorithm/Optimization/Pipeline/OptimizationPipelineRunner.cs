using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Pipeline;

/// <summary>
/// 优化算法流水线执行器
///
/// 按配置顺序执行多个优化算法
/// </summary>
public class OptimizationPipelineRunner
{
    private readonly SchedulingAlgorithmOptions options;

    private readonly Func<
        OptimizationAlgorithmType,
        ISolutionOptimizer> optimizerFactory;


    public OptimizationPipelineRunner(
        SchedulingAlgorithmOptions options,
        Func<
            OptimizationAlgorithmType,
            ISolutionOptimizer> optimizerFactory)
    {
        this.options =
            options;

        this.optimizerFactory =
            optimizerFactory;
    }


    /// <summary>
    /// 执行优化流水线
    /// </summary>
    public OptimizationResult Run(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var current =
            new OptimizationResult
            {
                Solution =
                    solution,

                Timelines =
                    timelines,

                Evaluation =
                    evaluator.Evaluate(
                        solution,
                        timelines,
                        context)
            };


        var steps =
            options.Pipeline
                .Where(x =>
                    x.Enabled)
                .OrderBy(x =>
                    x.Order)
                .ToList();



        foreach(var step in steps)
        {
            var optimizer =
                optimizerFactory(
                    step.Algorithm);



            current =
                optimizer.Optimize(
                    current.Solution,
                    context,
                    current.Timelines,
                    evaluator);
        }


        return current;
    }
}