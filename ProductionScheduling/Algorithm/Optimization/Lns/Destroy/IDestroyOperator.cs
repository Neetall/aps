using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Destroy;

public interface IDestroyOperator
{
    List<ScheduledOperation> Destroy(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        double rate);
}