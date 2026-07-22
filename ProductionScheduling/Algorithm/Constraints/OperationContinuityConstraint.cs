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

            ValidateNoPreemption(
                operation,
                solution,
                factory);
        }
    }

    private void ValidateNoPreemption(
        ScheduledOperation operation,
        SchedulingSolution solution,
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

            var inserted =
                solution.Operations
                    .Any(x =>
                        x != operation &&
                        x.FactoryCode == operation.FactoryCode &&
                        x.MachineCode == operation.MachineCode &&
                        x.StartSlot <= slot &&
                        x.EndSlot > slot);

            if(inserted)
            {
                throw new ConstraintViolationException(
                    Name,
                    $"工序被其他任务插入:{operation.JobTicketCode},Slot={slot}");
            }
        }
    }
}
