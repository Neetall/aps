using System.Diagnostics;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Application.Result;
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

        var algorithmResults =
            new List<OptimizationAlgorithmResult>();

        var current = new OptimizationResult
        {
            Solution = solution,
            Timelines = timelines,
            Evaluation = evaluator.Evaluate(
                solution,
                timelines,
                context),
            StartedAt = pipelineStartedAt,
            AlgorithmResults = algorithmResults
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

            CompletePipelineResult(
                current,
                pipelineStartedAt,
                pipelineWatch,
                false,
                algorithmResults);

            return current;
        }

        foreach(var step in steps)
        {
            if(IsTimedOut(
                   context,
                   pipelineWatch))
            {
                current.TimedOut =
                    true;

                Console.WriteLine(
                    $"优化超时，跳过剩余算法，已耗时:{FormatElapsed(pipelineWatch.Elapsed)}");

                break;
            }

            var algorithmStartedAt =
                DateTime.Now;

            var algorithmWatch =
                Stopwatch.StartNew();

            var beforeScore =
                current.Evaluation?.Score
                ?? double.MaxValue;

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

                algorithmResults.Add(
                    BuildAlgorithmResult(
                        step.Algorithm,
                        false,
                        false,
                        false,
                        beforeScore,
                        null,
                        algorithmStartedAt,
                        DateTime.Now,
                        algorithmWatch.Elapsed,
                        ex.Message));

                Console.WriteLine(
                    $"算法执行失败:{step.Algorithm}, 结束时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(algorithmWatch.Elapsed)}, {ex.Message}");

                continue;
            }

            algorithmWatch.Stop();

            if(result.Evaluation == null)
            {
                algorithmResults.Add(
                    BuildAlgorithmResult(
                        step.Algorithm,
                        false,
                        false,
                        false,
                        beforeScore,
                        null,
                        algorithmStartedAt,
                        DateTime.Now,
                        algorithmWatch.Elapsed,
                        "算法未返回评估结果"));

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
                algorithmResults.Add(
                    BuildAlgorithmResult(
                        step.Algorithm,
                        true,
                        true,
                        IsTimedOut(context,pipelineWatch),
                        beforeScore,
                        result.Evaluation.Score,
                        algorithmStartedAt,
                        DateTime.Now,
                        algorithmWatch.Elapsed,
                        "接受优化结果"));

                current = result;
                current.AlgorithmResults =
                    algorithmResults;

                Console.WriteLine(
                    $"接受优化结果:{step.Algorithm}");
            }
            else
            {
                algorithmResults.Add(
                    BuildAlgorithmResult(
                        step.Algorithm,
                        true,
                        false,
                        IsTimedOut(context,pipelineWatch),
                        beforeScore,
                        result.Evaluation.Score,
                        algorithmStartedAt,
                        DateTime.Now,
                        algorithmWatch.Elapsed,
                        "未优于当前方案"));

                Console.WriteLine(
                    $"拒绝优化结果:{step.Algorithm}");
            }
        }

        pipelineWatch.Stop();

        Console.WriteLine(
            $"优化结束 时间:{FormatTime(DateTime.Now)}, 耗时:{FormatElapsed(pipelineWatch.Elapsed)}, Score:{current.Evaluation.Score}");

        CompletePipelineResult(
            current,
            pipelineStartedAt,
            pipelineWatch,
            current.TimedOut,
            algorithmResults);

        return current;
    }

    private static bool IsTimedOut(
        SchedulingContext context,
        Stopwatch watch)
    {
        return context.ExecutionOptions.TimeoutSeconds > 0 &&
               watch.Elapsed.TotalSeconds >=
               context.ExecutionOptions.TimeoutSeconds;
    }

    private static OptimizationAlgorithmResult BuildAlgorithmResult(
        OptimizationAlgorithmType algorithm,
        bool success,
        bool accepted,
        bool timedOut,
        double beforeScore,
        double? afterScore,
        DateTime startedAt,
        DateTime endedAt,
        TimeSpan elapsed,
        string? message)
    {
        double? improvement =
            afterScore.HasValue
                ? beforeScore -
                  afterScore.Value
                : null;

        double? improvementRate =
            improvement.HasValue &&
            beforeScore > 0 &&
            beforeScore < double.MaxValue
                ? improvement.Value /
                  beforeScore
                : null;

        return new OptimizationAlgorithmResult
        {
            Algorithm = algorithm,
            Success = success,
            Accepted = accepted,
            TimedOut = timedOut,
            BeforeScore = beforeScore,
            AfterScore = afterScore,
            Improvement = improvement,
            ImprovementRate = improvementRate,
            StartedAt = startedAt,
            EndedAt = endedAt,
            ElapsedMilliseconds = elapsed.TotalMilliseconds,
            Message = message
        };
    }

    private static void CompletePipelineResult(
        OptimizationResult result,
        DateTime startedAt,
        Stopwatch watch,
        bool timedOut,
        List<OptimizationAlgorithmResult> algorithmResults)
    {
        result.StartedAt =
            startedAt;

        result.EndedAt =
            DateTime.Now;

        result.ElapsedMilliseconds =
            watch.Elapsed.TotalMilliseconds;

        result.TimedOut =
            timedOut ||
            algorithmResults.Any(x => x.TimedOut);

        result.AlgorithmResults =
            algorithmResults;
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
