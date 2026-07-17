using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.core;

public class LnsState
{
    public SchedulingSolution Solution { get; set; } = null!;


    public TimelineContext Timeline { get; set; } = null!;


    public EvaluationResult? Evaluation { get; set; }
}