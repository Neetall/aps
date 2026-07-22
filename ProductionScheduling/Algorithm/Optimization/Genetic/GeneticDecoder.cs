using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

public sealed class GeneticDecoder
{
    private readonly TimelineInitializer timelineInitializer;
    private readonly SchedulePlacementService placementService;

    public GeneticDecoder(
        TimelineInitializer timelineInitializer,
        SchedulePlacementService placementService)
    {
        this.timelineInitializer =
            timelineInitializer;

        this.placementService =
            placementService;
    }

    public void Decode(
        GeneticIndividual individual,
        SchedulingContext context,
        ScheduleEvaluator evaluator)
    {
        ArgumentNullException.ThrowIfNull(
            individual);

        ArgumentNullException.ThrowIfNull(
            context);

        ArgumentNullException.ThrowIfNull(
            evaluator);

        var timelines =
            timelineInitializer.Initialize(
                context);

        var solution =
            new SchedulingSolution();

        var orderTickets =
            context.Orders
                .ToDictionary(
                    x => x.Code,
                    x => x.JobTickets
                        .OrderBy(y =>
                            y.Sequence)
                        .ThenBy(y =>
                            y.Code,
                            StringComparer.OrdinalIgnoreCase)
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase);

        var orderIndexes =
            context.Orders
                .ToDictionary(
                    x => x.Code,
                    _ => 0,
                    StringComparer.OrdinalIgnoreCase);

        var orderEndSlots =
            context.Orders
                .ToDictionary(
                    x => x.Code,
                    _ => 0,
                    StringComparer.OrdinalIgnoreCase);

        while(true)
        {
            var next =
                context.Orders
                    .Select(order =>
                        CreateNextTicket(
                            order,
                            orderTickets,
                            orderIndexes))
                    .Where(x =>
                        x.Ticket != null)
                    .OrderBy(x =>
                        GetPriorityGene(
                            individual,
                            x.Ticket!.Code))
                    .ThenBy(x =>
                        x.Order.Priority)
                    .ThenBy(x =>
                        x.Ticket!.Sequence)
                    .ThenBy(x =>
                        x.Ticket!.Code,
                        StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault();

            if(next.Ticket == null)
                break;

            var ticket =
                next.Ticket;

            var candidate =
                placementService.FindPreferredMachine(
                    ticket,
                    timelines,
                    GetMachineGene(
                        individual,
                        ticket.Code),
                    orderEndSlots[
                        next.Order.Code]);

            if(candidate == null)
            {
                solution.IsFeasible =
                    false;

                MarkRemainingUnscheduled(
                    solution,
                    next.Order,
                    orderTickets,
                    orderIndexes);

                continue;
            }

            placementService.Commit(
                solution,
                candidate);

            orderEndSlots[
                next.Order.Code] =
                candidate.EndSlot;

            orderIndexes[
                next.Order.Code]++;
        }

        solution.IsFeasible =
            solution.UnscheduledJobTickets.Count == 0;

        individual.Solution =
            solution;

        individual.Timelines =
            timelines;

        individual.Evaluation =
            evaluator.Evaluate(
                solution,
                timelines,
                context);
    }

    private (Order Order, JobTicket? Ticket) CreateNextTicket(
        Order order,
        IReadOnlyDictionary<
            string,
            List<JobTicket>> orderTickets,
        IReadOnlyDictionary<string,int> orderIndexes)
    {
        var tickets =
            orderTickets[
                order.Code];

        var index =
            orderIndexes[
                order.Code];

        return index >= tickets.Count
            ? (order,null)
            : (order,tickets[index]);
    }

    private void MarkRemainingUnscheduled(
        SchedulingSolution solution,
        Order order,
        IReadOnlyDictionary<
            string,
            List<JobTicket>> orderTickets,
        IDictionary<string,int> orderIndexes)
    {
        var tickets =
            orderTickets[
                order.Code];

        var index =
            orderIndexes[
                order.Code];

        for(var current = index;
            current < tickets.Count;
            current++)
        {
            solution.UnscheduledJobTickets.Add(
                $"{tickets[current].Code}:没有可用设备或设备时间不足");
        }

        orderIndexes[
            order.Code] =
            tickets.Count;
    }

    private double GetPriorityGene(
        GeneticIndividual individual,
        string code)
    {
        return individual.PriorityGenes.TryGetValue(
            code,
            out var value)
            ? value
            : double.MaxValue;
    }

    private string? GetMachineGene(
        GeneticIndividual individual,
        string jobTicketCode)
    {
        return individual.MachineGenes.TryGetValue(
            jobTicketCode,
            out var machineCode)
            ? machineCode
            : null;
    }
}
