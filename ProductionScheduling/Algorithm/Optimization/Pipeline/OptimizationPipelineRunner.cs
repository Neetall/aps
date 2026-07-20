using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Pipeline;

/// <summary>
/// 优化算法流水线执行器
///
/// 按配置顺序执行多个优化算法
///
/// 示例:
///
/// Greedy
///   ↓
/// LocalSearch
///   ↓
/// SimulatedAnnealing
///   ↓
/// Tabu
///   ↓
/// LNS
///
/// 每一步都会尝试改善当前最佳方案
/// </summary>
public class OptimizationPipelineRunner
{
    private readonly SchedulingAlgorithmOptions options;

    private readonly OptimizerFactory optimizerFactory;



    public OptimizationPipelineRunner(
        SchedulingAlgorithmOptions options,
        OptimizerFactory optimizerFactory)
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



        Console.WriteLine(
            $"优化开始 Score:{current.Evaluation.Score}");



        var steps =
            options.Pipeline
                .Where(x =>
                    x.Enabled)
                .OrderBy(x =>
                    x.Order)
                .ToList();



        foreach(var step in steps)
        {
            Console.WriteLine(
                $"执行优化算法:{step.Algorithm}");



            var optimizer =
                optimizerFactory.Create(
                    step.Algorithm);



            var result =
                optimizer.Optimize(
                    current.Solution,
                    context,
                    current.Timelines,
                    evaluator);



            if(result.Evaluation == null)
            {
                continue;
            }



            Console.WriteLine(
                $"算法:{step.Algorithm}, Score:{result.Evaluation.Score}");



            /*
             * 只接受更优方案
             *
             * 防止:
             * SA/LNS随机搜索导致结果变差
             */
            if(result.Evaluation.Score <
               current.Evaluation.Score)
            {
                current =
                    result;


                Console.WriteLine(
                    $"接受优化结果:{step.Algorithm}");
            }
            else
            {
                Console.WriteLine(
                    $"拒绝优化结果:{step.Algorithm}");
            }
        }



        Console.WriteLine(
            $"优化结束 Score:{current.Evaluation.Score}");



        return current;
    }
}