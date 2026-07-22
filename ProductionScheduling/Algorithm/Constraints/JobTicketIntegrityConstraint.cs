using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Constraints;

public sealed class JobTicketIntegrityConstraint
    : ISchedulingConstraint
{
    public string Name =>
        "JobTicketIntegrity";

    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        ValidateDuplicateJobTicket(
            solution);

        ValidateJobTicketExist(
            solution,
            context);
    }

    private void ValidateDuplicateJobTicket(
        SchedulingSolution solution)
    {
        var duplicate =
            solution.Operations
                .GroupBy(x =>
                    x.JobTicketCode)
                .FirstOrDefault(x =>
                    x.Count() > 1);

        if(duplicate != null)
        {
            throw new ConstraintViolationException(
                Name,
                $"JobTicket重复:{duplicate.Key}");
        }
    }

    private void ValidateJobTicketExist(
        SchedulingSolution solution,
        SchedulingContext context)
    {
        var tickets =
            context.Orders
                .SelectMany(x =>
                    x.JobTickets)
                .Select(x =>
                    x.Code)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

        foreach(var operation in solution.Operations)
        {
            if(!tickets.Contains(
                   operation.JobTicketCode))
            {
                throw new ConstraintViolationException(
                    Name,
                    $"JobTicket不存在:{operation.JobTicketCode}");
            }
        }
    }
}
