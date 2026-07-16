using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 局部搜索优化器
///
/// 基于邻域移动不断寻找更优排产方案
/// </summary>
public class LocalSearchOptimizer : ISolutionOptimizer
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly OperationSelector operationSelector;

    private readonly MoveSelector moveSelector;

    private readonly SolutionCloner cloner;

    private readonly int iterations;



    public LocalSearchOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        int iterations = 1000)
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

        this.iterations =
            iterations;
    }



    /// <summary>
    /// 执行局部搜索
    /// </summary>
    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator)
    {
        /*
         * 初始状态复制
         *
         * 防止修改外部传入对象
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



        for(var i = 0; i < iterations; i++)
        {
            /*
             * 创建候选方案
             */
            var candidate =
                cloner.Clone(
                    current);



            /*
             * 选择优化目标
             */
            var operation =
                operationSelector.Select(
                    candidate.Solution);



            if(operation == null)
            {
                continue;
            }



            /*
             * 选择邻域Move
             */
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
             * 执行移动
             */
            var success =
                move.Apply(
                    moveContext);



            if(!success)
            {
                continue;
            }



            /*
             * 重新评价
             */
            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timeline,
                    context);



            /*
             * 只接受更优方案
             */
            if(candidate.Evaluation.Score <
               current.Evaluation!.Score)
            {
                current =
                    candidate;
            }
        }



        return new OptimizationResult
        {
            Solution =
                current.Solution,

            Timeline =
                current.Timeline,

            Evaluation =
                current.Evaluation
        };
    }
}