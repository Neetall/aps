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

    private readonly MoveNeighborhoodGenerator neighborhoodGenerator;

    private readonly SolutionCloner cloner;

    private readonly TabuSearchOptions options;


    public TabuSearchOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        MoveNeighborhoodGenerator neighborhoodGenerator,
        SolutionCloner cloner,
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


        for (var iteration = 0;
             iteration < options.Iterations;
             iteration++)
        {
            var neighbors =
                neighborhoodGenerator.Generate(
                    current.Solution);


            MoveCandidate? bestCandidate =
                null;


            foreach (var neighbor in neighbors)
            {
                var candidate =
                    cloner.Clone(
                        current);


                var operation =
                    candidate.Solution
                        .Operations
                        .First(x =>
                            x.JobTicketCode ==
                            neighbor.Operation.JobTicketCode);


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


                if (!neighbor.Move.Apply(
                        moveContext))
                    continue;


                candidate.Evaluation =
                    evaluator.Evaluate(
                        candidate.Solution,
                        candidate.Timeline,
                        context);


                var record =
                    moveContext.ExecutionRecord;


                if (record == null ||
                    !record.Success)
                    continue;


                var tabuKey =
                    record.TabuKey;


                var isTabu =
                    tabuKey != null &&
                    tabu.IsTabu(
                        tabuKey,
                        iteration);


                /*
                 * Aspiration:
                 *
                 * 如果超过历史最好
                 * 忽略Tabu
                 */
                var aspiration =
                    candidate.Evaluation.Score <
                    best.Evaluation!.Score;


                if (isTabu &&
                    !aspiration)
                    continue;


                /*
                 * 如果不允许接受差解
                 */
                if (!options.AllowWorseMoves &&
                    candidate.Evaluation.Score >=
                    current.Evaluation!.Score)
                    continue;


                if (bestCandidate == null ||
                    candidate.Evaluation.Score <
                    bestCandidate.Score)
                    bestCandidate =
                        new MoveCandidate
                        {
                            Operation =
                                neighbor.Operation,

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


            /*
             * 没有可接受邻居
             */
            if (bestCandidate == null)
                continue;


            current =
                bestCandidate.State!;


            /*
             * 加入Tabu
             */
            if (bestCandidate.Record?.TabuKey != null)
                tabu.Add(
                    bestCandidate.Record.TabuKey,
                    iteration);


            /*
             * 更新全局最优
             */
            if (current.Evaluation!.Score <
                best.Evaluation!.Score)
                best =
                    cloner.Clone(
                        current);
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