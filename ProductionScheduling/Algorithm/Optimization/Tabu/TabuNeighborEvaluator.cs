using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Tabu;

public class TabuNeighborEvaluator
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly SolutionCloner cloner;


    public TabuNeighborEvaluator(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        SolutionCloner cloner)
    {
        this.resourceIndex =
            resourceIndex;

        this.jobTicketIndex =
            jobTicketIndex;

        this.cloner =
            cloner;
    }



    public ScheduleState? Evaluate(
        MoveCandidate candidate,
        ScheduleState current,
        SchedulingContext context,
        ScheduleEvaluator evaluator)
    {
        var state =
            cloner.Clone(
                current);



        var moveContext =
            new MoveContext
            {
                SchedulingContext =
                    context,

                Solution =
                    state.Solution,

                Timeline =
                    state.Timeline,

                ResourceIndex =
                    resourceIndex,

                JobTicketIndex =
                    jobTicketIndex,

                CurrentOperation =
                    candidate.Operation
            };



        var success =
            candidate.Move.Apply(
                moveContext);


        if(!success)
            return null;



        state.Evaluation =
            evaluator.Evaluate(
                state.Solution,
                state.Timeline,
                context);



        return state;
    }
}