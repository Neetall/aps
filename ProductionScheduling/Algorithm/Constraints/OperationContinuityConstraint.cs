using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Constraints;

public sealed class OperationContinuityConstraint
    : ISchedulingConstraint
{
    public string Name =>
        "OperationContinuity";

    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        foreach(var operation in solution.Operations)
        {
            if(operation.StartSlot < 0)
            {
                throw new ConstraintViolationException(
                    Name,
                    $"开始Slot非法:{operation.JobTicketCode}");
            }

            if(operation.DurationSlots <= 0)
            {
                throw new ConstraintViolationException(
                    Name,
                    $"Duration非法:{operation.JobTicketCode}");
            }

            var factory =
                timelines.Get(
                    operation.FactoryCode);

            if(operation.EndSlot >
               factory.TimeModel.SlotCount)
            {
                throw new ConstraintViolationException(
                    Name,
                    $"超出时间轴范围:{operation.JobTicketCode}");
            }

            ValidateWorkingSlots(
                operation,
                factory);
        }
    }

    private void ValidateWorkingSlots(
        ScheduledOperation operation,
        FactoryTimeline factory)
    {
        for(var slot = operation.StartSlot;
            slot < operation.EndSlot;
            slot++)
        {
            if(!factory.TimeModel.IsWorkingSlot(
                   slot))
            {
                throw new ConstraintViolationException(
                    Name,
                    $"工序跨越非工作Slot:{operation.JobTicketCode},Slot={slot}");
            }
        }
    }
}
