using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Constraints;

public interface ISchedulingConstraint
{
    string Name { get; }

    void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines);
}
