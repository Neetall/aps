using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Constraints;

public sealed class OperationPrecedenceConstraint
    : ISchedulingConstraint
{
    public string Name =>
        "OperationPrecedence";

    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        var operationMap =
            solution.Operations
                .GroupBy(
                    x => x.JobTicketCode,
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.First(),
                    StringComparer.OrdinalIgnoreCase);

        foreach(var order in context.Orders)
        {
            var tickets =
                order.JobTickets
                    .OrderBy(x =>
                        x.Sequence)
                    .ThenBy(x =>
                        x.Code,
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            for(var index = 1;
                index < tickets.Count;
                index++)
            {
                var previousTicket =
                    tickets[index - 1];

                var currentTicket =
                    tickets[index];

                if(!operationMap.TryGetValue(
                       previousTicket.Code,
                       out var previousOperation) ||
                   !operationMap.TryGetValue(
                       currentTicket.Code,
                       out var currentOperation))
                {
                    continue;
                }

                if(currentOperation.StartSlot <
                   previousOperation.EndSlot)
                {
                    throw new ConstraintViolationException(
                        Name,
                        $"订单工序顺序冲突:Order={order.Code},Previous={previousTicket.Code},PreviousEnd={previousOperation.EndSlot},Current={currentTicket.Code},CurrentStart={currentOperation.StartSlot}");
                }
            }
        }
    }
}
