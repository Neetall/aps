using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;

/// <summary>
///     模拟退火优化器
///     在局部搜索基础上允许接受一定概率的差解
///     用于跳出局部最优
/// </summary>
public class SimulatedAnnealingOptimizer : ISolutionOptimizer
{
    private readonly AcceptanceCriteria acceptanceCriteria;

    private readonly SolutionCloner cloner;

    private readonly double coolingRate;

    private readonly double initialTemperature;

    private readonly int iterations;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly MoveSelector moveSelector;

    private readonly OperationSelector operationSelector;
    private readonly SchedulingResourceIndex resourceIndex;


    public SimulatedAnnealingOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        AcceptanceCriteria acceptanceCriteria,
        int iterations = 10000,
        double initialTemperature = 1000,
        double coolingRate = 0.95)
    {
        this.resourceIndex =
            resourceIndex;

        this.jobTicketIndex =
            jobTicketIndex;

        this.operationSelector =
            operationSelector;

        this.moveSelector =
            moveSelector;

        this.cloner =
            cloner;

        this.acceptanceCriteria =
            acceptanceCriteria;

        this.iterations =
            iterations;

        this.initialTemperature =
            initialTemperature;

        this.coolingRate =
            coolingRate;
    }


    /// <summary>
    ///     执行模拟退火
    /// </summary>
    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator)
    {
        /*
         * 当前状态
         */
        var current =
            cloner.Clone(
                new ScheduleState
                {
                    Solution =
                        solution,

                    Timeline =
                        timeline
                });


        current.Evaluation =
            evaluator.Evaluate(
                current.Solution,
                current.Timeline,
                context);


        /*
         * 全局最优
         */
        var best =
            cloner.Clone(
                current);


        var temperature =
            initialTemperature;


        for (var i = 0; i < iterations; i++)
        {
            /*
             * 创建候选
             */
            var candidate =
                cloner.Clone(
                    current);


            var operation =
                operationSelector.Select(
                    candidate.Solution);


            if (operation == null) continue;


            var move =
                moveSelector.Select();


            var moveContext =
                new MoveContext
                {
                    SchedulingContext =
                        context,

                    Solution =
                        candidate.Solution,

                    Timeline =
                        candidate.Timeline,

                    ResourceIndex =
                        resourceIndex,

                    JobTicketIndex =
                        jobTicketIndex,

                    CurrentOperation =
                        operation
                };


            /*
             * 保存移动前评分
             */
            var oldScore =
                current.Evaluation!.Score;


            var success =
                move.Apply(
                    moveContext);


            if (!success)
            {
                temperature =
                    CoolDown(
                        temperature);

                continue;
            }


            /*
             * 评价新方案
             */
            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timeline,
                    context);


            var newScore =
                candidate.Evaluation.Score;


            /*
             * SA接受判断
             */
            var accepted =
                acceptanceCriteria.Accept(
                    oldScore,
                    newScore,
                    temperature);


            /*
             * 写入Move记录
             */
            candidate.History.Add(
                new MoveExecutionRecord
                {
                    MoveName =
                        move.Name,

                    Success =
                        true,

                    OldScore =
                        oldScore,

                    NewScore =
                        newScore,

                    Accepted =
                        accepted,

                    JobTicketCode =
                        operation.JobTicketCode
                });


            if (accepted)
                current =
                    candidate;


            /*
             * 更新最好解
             */
            if (current.Evaluation!.Score <
                best.Evaluation!.Score)
                best =
                    cloner.Clone(
                        current);


            temperature =
                CoolDown(
                    temperature);


            if (temperature < 0.1) break;
        }


        return new OptimizationResult
        {
            Solution =
                best.Solution,

            Timeline =
                best.Timeline,

            Evaluation =
                best.Evaluation
        };
    }


    private double CoolDown(
        double temperature)
    {
        return temperature *
               coolingRate;
    }
}