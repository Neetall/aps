using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Repair;

public interface IRepairOperator
{
    void Repair(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        List<ScheduledOperation> removed);
}