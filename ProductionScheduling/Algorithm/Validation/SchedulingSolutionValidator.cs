using ProductionScheduling.Algorithm.Constraints;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Validation;

public class SchedulingSolutionValidator
{
    private readonly IReadOnlyList<ISchedulingConstraint> constraints;

    public SchedulingSolutionValidator()
        : this(
            [
                new JobTicketIntegrityConstraint(),
                new MachineEligibilityConstraint(),
                new OperationPrecedenceConstraint(),
                new OperationContinuityConstraint(),
                new MachineCapacityConstraint()
            ])
    {
    }

    public SchedulingSolutionValidator(
        IEnumerable<ISchedulingConstraint> constraints)
    {
        this.constraints =
            constraints.ToList();
    }

    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        foreach(var constraint in constraints)
        {
            constraint.Validate(
                solution,
                context,
                timelines);
        }
    }
}
