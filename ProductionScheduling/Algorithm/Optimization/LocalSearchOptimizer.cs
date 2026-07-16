using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 局部搜索优化器
///
/// 不断尝试邻域移动
/// 保留评分更优的方案
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
        this.resourceIndex = resourceIndex;

        this.jobTicketIndex = jobTicketIndex;

        this.operationSelector = operationSelector;

        this.moveSelector = moveSelector;

        this.cloner = cloner;

        this.iterations = iterations;
    }



    /// <summary>
    /// 执行局部搜索
    /// </summary>
    public SchedulingSolution Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator)
    {
        var current =
            new ScheduleState
            {
                Solution =
                    solution,

                Timeline =
                    timeline,

                Evaluation =
                    evaluator.Evaluate(
                        solution,
                        timeline,
                        context)
            };



        for(var i = 0; i < iterations; i++)
        {
            var candidate =
                cloner.Clone(
                    current);



            var operation =
                operationSelector.Select(
                    candidate.Solution);


            if(operation == null)
                continue;



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



            var success =
                move.Apply(
                    moveContext);



            if(!success)
                continue;



            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timeline,
                    context);



            if(candidate.Evaluation.Score <
               current.Evaluation.Score)
            {
                current =
                    candidate;
            }
        }


        return current.Solution;
    }
}