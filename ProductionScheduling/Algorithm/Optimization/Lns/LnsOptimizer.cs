using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.core;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns;

public class LnsOptimizer : ISolutionOptimizer
{
    private readonly SolutionCloner cloner;

    private readonly IDestroyOperator destroyOperator;

    private readonly IRepairOperator repairOperator;

    private readonly ILnsAcceptance acceptance;

    private readonly SchedulingSolutionValidator validator;

    private readonly LnsOptions options;


    public LnsOptimizer(
        SolutionCloner cloner,
        IDestroyOperator destroyOperator,
        IRepairOperator repairOperator,
        ILnsAcceptance acceptance,
        SchedulingSolutionValidator validator,
        LnsOptions options)
    {
        this.cloner =
            cloner;

        this.destroyOperator =
            destroyOperator;

        this.repairOperator =
            repairOperator;

        this.acceptance =
            acceptance;

        this.validator =
            validator;

        this.options =
            options;
    }


    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var current =
            new LnsState
            {
                Solution =
                    cloner.CloneSolution(
                        solution),

                Timelines =
                    cloner.CloneTimelines(
                        timelines),

                Evaluation =
                    evaluator.Evaluate(
                        solution,
                        timelines,
                        context)
            };


        var best =
            cloner.Clone(
                current);



        for(var i = 0;
            i < options.Iterations;
            i++)
        {
            var candidate =
                cloner.Clone(
                    current);



            var removed =
                destroyOperator.Destroy(
                    candidate.Solution,
                    candidate.Timelines,
                    options.DestroyRate);



            if(removed.Count == 0)
                continue;



            repairOperator.Repair(
                candidate.Solution,
                candidate.Timelines,
                removed);



            if(!IsValid(
                    candidate.Solution,
                    context,
                    candidate.Timelines))
            {
                continue;
            }



            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timelines,
                    context);



            if(!acceptance.Accept(
                    current.Evaluation!,
                    candidate.Evaluation))
            {
                continue;
            }



            current =
                candidate;



            if(current.Evaluation!.Score <
               best.Evaluation!.Score)
            {
                best =
                    cloner.Clone(
                        current);
            }
        }



        return new OptimizationResult
        {
            Solution =
                best.Solution,

            Timelines =
                best.Timelines,

            Evaluation =
                best.Evaluation
        };
    }



    private bool IsValid(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        try
        {
            validator.Validate(
                solution,
                context,
                timelines);

            return true;
        }
        catch
        {
            return false;
        }
    }
}