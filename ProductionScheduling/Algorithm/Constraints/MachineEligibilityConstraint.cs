using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Constraints;

public sealed class MachineEligibilityConstraint
    : ISchedulingConstraint
{
    public string Name =>
        "MachineEligibility";

    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        ValidateMachineExist(
            solution,
            timelines);

        ValidateMachineCapability(
            solution,
            context);
    }

    private void ValidateMachineExist(
        SchedulingSolution solution,
        TimelineContextGroup timelines)
    {
        foreach(var operation in solution.Operations)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);

            if(!factory.Machines.ContainsKey(
                   operation.MachineCode))
            {
                throw new ConstraintViolationException(
                    Name,
                    $"设备不存在:{operation.FactoryCode}/{operation.MachineCode}");
            }
        }
    }

    private void ValidateMachineCapability(
        SchedulingSolution solution,
        SchedulingContext context)
    {
        foreach(var operation in solution.Operations)
        {
            var machine =
                context.Machines
                    .FirstOrDefault(x =>
                        x.Code ==
                        operation.MachineCode);

            if(machine == null)
                continue;

            var capable =
                machine.Capabilities
                    .Any(x =>
                        string.Equals(
                            x.JobTicketCode,
                            operation.JobTicketCode,
                            StringComparison.OrdinalIgnoreCase));

            if(!capable)
            {
                throw new ConstraintViolationException(
                    Name,
                    $"设备无加工能力:Machine={operation.MachineCode},JobTicket={operation.JobTicketCode}");
            }
        }
    }
}
