using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Destroy;


public class WorstScoreDestroyOperator : IDestroyOperator
{
    private readonly ScheduleEvaluator evaluator;

    private readonly SchedulingContext context;



    public WorstScoreDestroyOperator(
        ScheduleEvaluator evaluator,
        SchedulingContext context)
    {
        this.evaluator =
            evaluator;

        this.context =
            context;
    }



    public List<ScheduledOperation> Destroy(
        SchedulingSolution solution,
        TimelineContext timeline,
        double rate)
    {
        if(solution.Operations.Count == 0)
            return [];



        var removeCount =
            Math.Max(
                1,
                (int)Math.Ceiling(
                    solution.Operations.Count *
                    rate));



        var baseEvaluation =
            evaluator.Evaluate(
                solution,
                timeline,
                context);



        var scores =
            new List<OperationScore>();



        /*
         * 计算每个任务影响
         */
        foreach(var operation in
                solution.Operations.ToList())
        {
            solution.Operations.Remove(
                operation);



            var evaluation =
                evaluator.Evaluate(
                    solution,
                    timeline,
                    context);



            solution.Operations.Add(
                operation);



            scores.Add(
                new OperationScore
                {
                    Operation =
                        operation,

                    Score =
                        evaluation.Score -
                        baseEvaluation.Score
                });
        }



        var removed =
            scores
                .OrderByDescending(x =>
                    x.Score)
                .Take(removeCount)
                .Select(x =>
                    x.Operation)
                .ToList();



        /*
         * 真正Destroy
         *
         * 1. Solution删除
         * 2. Timeline释放
         */
        foreach(var operation in removed)
        {
            solution.Operations.Remove(
                operation);



            if(timeline.Machines
                    .TryGetValue(
                        operation.MachineCode,
                        out var machineTimeline))
            {
                machineTimeline.Release(
                    operation.StartSlot,
                    operation.DurationSlots);
            }
        }



        return removed;
    }



    private class OperationScore
    {
        public ScheduledOperation Operation { get; set; } = null!;


        public double Score { get; set; }
    }
}