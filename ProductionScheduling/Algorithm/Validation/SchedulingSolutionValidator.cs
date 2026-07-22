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
            CreateDefaultConstraints())
    {
    }

    public SchedulingSolutionValidator(
        IEnumerable<ISchedulingConstraint> constraints)
    {
        var constraintList =
            constraints.ToList();

        this.constraints =
            constraintList.Count > 0
                ? constraintList
                : CreateDefaultConstraints();
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

    private static IReadOnlyList<ISchedulingConstraint> CreateDefaultConstraints()
    {
        return
        [
            new JobTicketIntegrityConstraint(),
            new MachineEligibilityConstraint(),
            new OperationPrecedenceConstraint(),
            new OperationContinuityConstraint(),
            new MachineCapacityConstraint()
        ];
    }
}
