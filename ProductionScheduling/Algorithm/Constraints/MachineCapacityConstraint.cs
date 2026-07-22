using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Constraints;

public sealed class MachineCapacityConstraint
    : ISchedulingConstraint
{
    public string Name =>
        "MachineCapacity";

    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        ValidateMachineConflict(
            solution);

        ValidateTimelineConsistency(
            solution,
            timelines);
    }

    private void ValidateMachineConflict(
        SchedulingSolution solution)
    {
        foreach(var group in solution.Operations
                    .GroupBy(x =>
                        new
                        {
                            x.FactoryCode,
                            x.MachineCode
                        }))
        {
            var operations =
                group
                    .OrderBy(x =>
                        x.StartSlot)
                    .ToList();

            for(var i = 1;
                i < operations.Count;
                i++)
            {
                var previous =
                    operations[i - 1];

                var current =
                    operations[i];

                if(current.StartSlot <
                   previous.EndSlot)
                {
                    throw new ConstraintViolationException(
                        Name,
                        $"设备任务时间冲突:Factory={group.Key.FactoryCode},Machine={group.Key.MachineCode},Job1={previous.JobTicketCode},Job2={current.JobTicketCode}");
                }
            }
        }
    }

    private void ValidateTimelineConsistency(
        SchedulingSolution solution,
        TimelineContextGroup timelines)
    {
        foreach(var operation in solution.Operations)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);

            var machine =
                factory.Machines[
                    operation.MachineCode];

            for(var slot = operation.StartSlot;
                slot < operation.EndSlot;
                slot++)
            {
                if(machine.IsFree(slot))
                {
                    throw new ConstraintViolationException(
                        Name,
                        $"Timeline状态不一致:JobTicket={operation.JobTicketCode},Machine={operation.MachineCode},Slot={slot}");
                }
            }
        }
    }
}
