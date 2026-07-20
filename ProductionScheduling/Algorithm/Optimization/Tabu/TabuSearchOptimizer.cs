using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Tabu;

public class TabuSearchOptimizer : ISolutionOptimizer
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly MoveNeighborhoodGenerator neighborhoodGenerator;

    private readonly SolutionCloner cloner;

    private readonly SchedulingSolutionValidator validator;

    private readonly TabuSearchOptions options;


    public TabuSearchOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        MoveNeighborhoodGenerator neighborhoodGenerator,
        SolutionCloner cloner,
        SchedulingSolutionValidator validator,
        TabuSearchOptions options)
    {
        this.resourceIndex =
            resourceIndex;

        this.jobTicketIndex =
            jobTicketIndex;

        this.neighborhoodGenerator =
            neighborhoodGenerator;

        this.cloner =
            cloner;

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
            cloner.Clone(
                new ScheduleState
                {
                    Solution =
                        solution,

                    Timelines =
                        timelines
                });



        current.Evaluation =
            evaluator.Evaluate(
                current.Solution,
                current.Timelines,
                context);



        var best =
            cloner.Clone(
                current);



        var tabu =
            new TabuMemory(
                options.TabuTenure);



        for(var iteration = 0;
            iteration < options.Iterations;
            iteration++)
        {
            var neighbors =
                neighborhoodGenerator.Generate(
                    current.Solution);



            MoveCandidate? bestNeighbor =
                null;



            foreach(var neighbor in neighbors)
            {
                var candidate =
                    cloner.Clone(
                        current);



                var operation =
                    candidate.Solution
                        .Operations
                        .FirstOrDefault(x =>
                            x.JobTicketCode ==
                            neighbor.Operation.JobTicketCode);



                if(operation == null)
                    continue;



                var moveContext =
                    new MoveContext
                    {
                        SchedulingContext =
                            context,

                        Solution =
                            candidate.Solution,

                        Timelines =
                            candidate.Timelines,

                        ResourceIndex =
                            resourceIndex,

                        JobTicketIndex =
                            jobTicketIndex,

                        CurrentOperation =
                            operation
                    };



                if(!neighbor.Move.Apply(
                        moveContext))
                {
                    continue;
                }



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



                var record =
                    moveContext.ExecutionRecord;



                if(record == null ||
                   !record.Success)
                {
                    continue;
                }



                var tabuKey =
                    record.TabuKey;



                var isTabu =
                    tabuKey != null &&
                    tabu.IsTabu(
                        tabuKey,
                        iteration);



                var aspiration =
                    candidate.Evaluation.Score <
                    best.Evaluation!.Score;



                if(isTabu &&
                   !aspiration)
                {
                    continue;
                }



                if(!options.AllowWorseMoves &&
                   candidate.Evaluation.Score >=
                   current.Evaluation!.Score)
                {
                    continue;
                }



                if(bestNeighbor == null ||
                   candidate.Evaluation.Score <
                   bestNeighbor.Score)
                {
                    bestNeighbor =
                        new MoveCandidate
                        {
                            Operation =
                                operation,

                            Move =
                                neighbor.Move,

                            State =
                                candidate,

                            Record =
                                record,

                            Score =
                                candidate.Evaluation.Score
                        };
                }
            }



            if(bestNeighbor == null)
                continue;



            current =
                bestNeighbor.State!;



            if(bestNeighbor.Record?.TabuKey != null)
            {
                tabu.Add(
                    bestNeighbor.Record.TabuKey,
                    iteration);
            }



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