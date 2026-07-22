using System.Diagnostics;
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
/// 按配置顺序执行多个优化算法
/// </summary>
public class OptimizationPipelineRunner
{
    private readonly SchedulingAlgorithmOptions options;
    private readonly OptimizerFactory optimizerFactory;

    public OptimizationPipelineRunner(
        SchedulingAlgorithmOptions options,
        OptimizerFactory optimizerFactory)
    {
        this.options = options;
        this.optimizerFactory = optimizerFactory;
    }

    /// <summary>
    /// 执行优化流水线
    /// </summary>
    public OptimizationResult Run(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator,
        IReadOnlyCollection<OptimizationAlgorithmType>? algorithms = null)
    {
        var pipelineStartedAt =
            DateTime.Now;

        var pipelineWatch =
            Stopwatch.StartNew();

        var current = new OptimizationResult
        {
            Solution = solution,
            Timelines = timelines,
            Evaluation = evaluator.Evaluate(
                solution,
                timelines,
                context)
        };

        Console.WriteLine(
            $"优化开始 时间:{FormatTime(pipelineStartedAt)}, Score:{current.Evaluation.Score}");

        Console.WriteLine(
            $"请求算法:{FormatAlgorithms(algorithms)}");

        var hasRequestedAlgorithms =
            algorithms != null &&
            algorithms.Count > 0;

        var steps = options.Pipeline
            .Where(x =>
                hasRequestedAlgorithms
                    ? algorithms!.Contains(
                        x.Algorithm)
                    : x.Enabled)
            .OrderBy(x => x.Order)
            .ToList();

        Console.WriteLine(
            $"实际算法:{string.Join(",",steps.Select(x => x.Algorithm))}");

        if(steps.Count == 0)
        {
            Console.WriteLine("没有匹配到可执行的优化算法");
            pipelineWatch.Stop();

            Console.WriteLine(
                $"优化结束 时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(pipelineWatch.Elapsed)}, Score:{current.Evaluation.Score}");

            return current;
        }

        foreach(var step in steps)
        {
            var algorithmStartedAt =
                DateTime.Now;

            var algorithmWatch =
                Stopwatch.StartNew();

            Console.WriteLine(
                $"执行优化算法:{step.Algorithm}, 开始时间:{FormatTime(algorithmStartedAt)}");

            OptimizationResult result;

            try
            {
                var optimizer = optimizerFactory.Create(
                    step.Algorithm);

                result = optimizer.Optimize(
                    current.Solution,
                    context,
                    current.Timelines,
                    evaluator);
            }
            catch(Exception ex)
            {
                algorithmWatch.Stop();

                Console.WriteLine(
                    $"算法执行失败:{step.Algorithm}, 结束时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(algorithmWatch.Elapsed)}, {ex.Message}");

                continue;
            }

            algorithmWatch.Stop();

            if(result.Evaluation == null)
            {
                Console.WriteLine(
                    $"算法未返回评估结果:{step.Algorithm}, 结束时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(algorithmWatch.Elapsed)}");

                continue;
            }

            Console.WriteLine(
                $"算法:{step.Algorithm}, 结束时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(algorithmWatch.Elapsed)}, Score:{result.Evaluation.Score}");

            /*
             * 只接受更优方案
             *
             * SA、Tabu、LNS内部允许探索较差解，
             * 但流水线最终只保留更优结果。
             */
            if(result.Evaluation.Score <
               current.Evaluation.Score)
            {
                current = result;

                Console.WriteLine(
                    $"接受优化结果:{step.Algorithm}");
            }
            else
            {
                Console.WriteLine(
                    $"拒绝优化结果:{step.Algorithm}");
            }
        }

        pipelineWatch.Stop();

        Console.WriteLine(
            $"优化结束 时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(pipelineWatch.Elapsed)}, Score:{current.Evaluation.Score}");

        return current;
    }

    private static string FormatAlgorithms(
        IReadOnlyCollection<OptimizationAlgorithmType>? algorithms)
    {
        if(algorithms == null)
            return "未传入";

        if(algorithms.Count == 0)
            return "空集合";

        return string.Join(",",algorithms);
    }

    private static string FormatTime(
        DateTime time)
    {
        return time.ToString(
            "yyyy-MM-dd HH:mm:ss.fff");
    }

    private static string FormatElapsed(
        TimeSpan elapsed)
    {
        return elapsed.TotalSeconds >= 1
            ? $"{elapsed.TotalSeconds:0.###}s"
            : $"{elapsed.TotalMilliseconds:0.###}ms";
    }
}
