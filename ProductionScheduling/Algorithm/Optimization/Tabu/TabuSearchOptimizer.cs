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

    private readonly AlgorithmDebugOptions debugOptions;



    public TabuSearchOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        MoveNeighborhoodGenerator neighborhoodGenerator,
        SolutionCloner cloner,
        SchedulingSolutionValidator validator,
        TabuSearchOptions options,
        AlgorithmDebugOptions debugOptions)
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

        this.debugOptions =
            debugOptions;
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



        PipelineLog(
            $"TabuSearch开始 Score:{current.Evaluation.Score}");



        var acceptedCount =
            0;


        var tabuSkipCount =
            0;


        var invalidCount =
            0;


        var noNeighborCount =
            0;

        var noImprovementCount =
            0;



        for(var iteration = 0;
            iteration < options.Iterations;
            iteration++)
        {
            var neighbors =
                SelectNeighborhood(
                    neighborhoodGenerator.Generate(
                        current.Solution));



            IterationLog(
                $"Iteration:{iteration}, 邻域数量:{neighbors.Count}");



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
                    invalidCount++;

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
                    tabuSkipCount++;

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
            {
                noNeighborCount++;

                continue;
            }



            current =
                bestNeighbor.State!;



            acceptedCount++;



            IterationLog(
                $"Iteration:{iteration}, " +
                $"接受:{bestNeighbor.Move.Name}, " +
                $"Score:{bestNeighbor.Score}");



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

                noImprovementCount =
                    0;

                PipelineLog(
                    $"Tabu发现更优方案 Score:{best.Evaluation.Score}");
            }
            else
            {
                noImprovementCount++;

                if(noImprovementCount >=
                   options.NoImprovementLimit)
                {
                    PipelineLog(
                        $"TabuSearch提前结束，无改善轮数:{noImprovementCount}");

                    break;
                }
            }
        }



        PipelineLog(
            $"TabuSearch结束 " +
            $"Score:{best.Evaluation.Score}, " +
            $"接受:{acceptedCount}, " +
            $"禁忌跳过:{tabuSkipCount}, " +
            $"非法:{invalidCount}, " +
            $"无候选:{noNeighborCount}");



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

    private List<MoveCandidate> SelectNeighborhood(
        List<MoveCandidate> neighbors)
    {
        if(options.MaxNeighborhoodSize <= 0 ||
           neighbors.Count <= options.MaxNeighborhoodSize)
        {
            return neighbors;
        }

        var lateOperations =
            neighbors
                .OrderByDescending(x =>
                    x.Operation.EndSlot)
                .Take(
                    Math.Max(
                        1,
                        options.MaxNeighborhoodSize / 2))
                .ToList();

        var sampled =
            neighbors
                .Except(
                    lateOperations)
                .OrderBy(_ =>
                    Random.Shared.Next())
                .Take(
                    options.MaxNeighborhoodSize -
                    lateOperations.Count);

        return lateOperations
            .Concat(
                sampled)
            .ToList();
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
        catch(Exception ex)
        {
            Debug(
                $"Validator失败:{ex.Message}");

            return false;
        }
    }



    private void PipelineLog(
        string message)
    {
        if(debugOptions.EnablePipelineLog)
        {
            Console.WriteLine(
                message);
        }
    }



    private void IterationLog(
        string message)
    {
        if(debugOptions.EnableIterationLog)
        {
            Console.WriteLine(
                message);
        }
    }



    private void Debug(
        string message)
    {
        if(debugOptions.EnableDebugLog)
        {
            Console.WriteLine(
                message);
        }
    }
}
