using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Tabu;

public class TabuSearchOptimizer : ISolutionOptimizer
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly OperationSelector operationSelector;

    private readonly MoveSelector moveSelector;

    private readonly SolutionCloner cloner;

    private readonly TabuSearchOptions options;



    public TabuSearchOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        TabuSearchOptions options)
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

        this.options =
            options;
    }



    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator)
    {
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



        var best =
            cloner.Clone(
                current);



        var tabu =
            new TabuMemory(
                options.TabuTenure);



        for(var i = 0;
            i < options.Iterations;
            i++)
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


            if(move == null)
                continue;



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



            var record =
                moveContext.ExecutionRecord;



            if(record == null)
                continue;



            var tabuKey =
                record.TabuKey;

            if(tabuKey == null)
                continue;

            var isTabu =
                tabu.IsTabu(
                    tabuKey,
                    i);



            /*
             * 破禁规则:
             *
             * 如果产生全局最好解
             * 即使Tabu也接受
             */
            var aspiration =
                candidate.Evaluation.Score <
                best.Evaluation!.Score;



            if(isTabu &&
               !aspiration)
            {
                continue;
            }



            /*
             * Tabu Search允许接受变差解
             *
             * 目的是跳出局部最优
             */
            if(!options.AllowWorseMoves
               &&
               candidate.Evaluation.Score >=
               current.Evaluation!.Score)
            {
                continue;
            }



            current =
                candidate;



            tabu.Add(
                tabuKey,
                i);



            if(current.Evaluation.Score <
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

            Timeline =
                best.Timeline,

            Evaluation =
                best.Evaluation
        };
    }

}