using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

public sealed class GeneticAlgorithmOptimizer
    : ISolutionOptimizer
{
    private readonly GeneticAlgorithmOptions options;

    public GeneticAlgorithmOptimizer(
        SchedulingAlgorithmOptions options)
    {
        this.options =
            options.GeneticAlgorithm;
    }

    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var evaluation =
            evaluator.Evaluate(
                solution,
                timelines,
                context);

        Console.WriteLine(
            $"GeneticAlgorithm开始 Score:{evaluation.Score}");

        /*
         * 第一阶段只完成流水线接入。
         * 后续实现:
         * 1. 初始化种群
         * 2. 选择
         * 3. 交叉
         * 4. 变异
         * 5. 修复不可行解
         * 6. 精英保留
         */
        return new OptimizationResult
        {
            Solution = solution,
            Timelines = timelines,
            Evaluation = evaluation
        };
    }
}