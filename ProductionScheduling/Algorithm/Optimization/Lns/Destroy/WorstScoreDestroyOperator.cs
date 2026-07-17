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
        TimelineContextGroup timelines,
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
                timelines,
                context);



        var scores =
            new List<OperationScore>();



        foreach(var operation in solution.Operations.ToList())
        {
            solution.Operations.Remove(
                operation);



            var evaluation =
                evaluator.Evaluate(
                    solution,
                    timelines,
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



        foreach(var operation in removed)
        {
            solution.Operations.Remove(
                operation);



            var factory =
                timelines.Get(
                    operation.FactoryCode);



            if(factory.Machines
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