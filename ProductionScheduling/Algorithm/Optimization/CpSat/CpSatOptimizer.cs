using Google.OrTools.Sat;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.CpSat;

public sealed class CpSatOptimizer
    : ISolutionOptimizer
{
    private readonly CpSatOptions options;

    public CpSatOptimizer(
        SchedulingAlgorithmOptions options)
    {
        this.options =
            options.CpSat;
    }

    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var initialEvaluation =
            evaluator.Evaluate(
                solution,
                timelines,
                context);

        Console.WriteLine(
            $"CpSat开始 Score:{initialEvaluation.Score}");

        var model =
            new CpModel();

        /*
         * 后续在这里建立:
         *
         * 1. 工单开始时间变量
         * 2. 工单结束时间变量
         * 3. 设备选择布尔变量
         * 4. OptionalIntervalVar
         * 5. 每台设备AddNoOverlap
         * 6. 工序前后关系
         * 7. 最小化Makespan
         */

        var solver =
            new CpSolver();

        solver.StringParameters =
            BuildSolverParameters();

        /*
         * 模型尚未建立，当前先原样返回。
         */
        return new OptimizationResult
        {
            Solution = solution,
            Timelines = timelines,
            Evaluation = initialEvaluation
        };
    }

    private string BuildSolverParameters()
    {
        var parameters =
            $"max_time_in_seconds:{options.MaxSolveSeconds} " +
            $"log_search_progress:{options.EnableSolverLog.ToString().ToLowerInvariant()}";

        if(options.WorkerCount > 0)
        {
            parameters +=
                $" num_search_workers:{options.WorkerCount}";
        }

        return parameters;
    }
}