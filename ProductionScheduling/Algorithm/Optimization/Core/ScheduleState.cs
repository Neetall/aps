using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

public class ScheduleState
{
    public SchedulingSolution Solution { get; set; }

    public TimelineContextGroup Timelines { get; set; }

    public EvaluationResult? Evaluation { get; set; }

    public List<MoveExecutionRecord> History { get; set; }
}