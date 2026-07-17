using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

public class MoveContext
{
    public SchedulingContext SchedulingContext { get; set; } = null!;


    public SchedulingSolution Solution { get; set; } = null!;


    public TimelineContextGroup Timelines { get; set; } = null!;


    public SchedulingResourceIndex ResourceIndex { get; set; } = null!;


    public JobTicketIndex JobTicketIndex { get; set; } = null!;


    public ScheduledOperation CurrentOperation { get; set; } = null!;


    public MoveExecutionRecord? ExecutionRecord { get; set; }
}