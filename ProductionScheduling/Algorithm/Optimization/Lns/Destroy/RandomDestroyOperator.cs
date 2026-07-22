using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Destroy;

public class RandomDestroyOperator : IDestroyOperator
{
    private readonly Random random;


    public RandomDestroyOperator(
        Random? random = null)
    {
        this.random =
            random ??
            new Random();
    }



    public List<ScheduledOperation> Destroy(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        SchedulingContext context,
        double rate)
    {
        if(solution.Operations.Count == 0)
            return [];



        var count =
            Math.Max(
                1,
                (int)(
                    solution.Operations.Count *
                    rate));



        var removed =
            SelectRemovedOperations(
                solution,
                count);



        foreach(var operation in removed)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);



            if(factory.TryGetMachine(
                   operation.MachineCode,
                   out var machine))
            {
                machine.Release(
                    operation.StartSlot,
                    operation.DurationSlots);
            }



            solution.Operations.Remove(
                operation);
        }



        return removed;
    }

    private List<ScheduledOperation> SelectRemovedOperations(
        SchedulingSolution solution,
        int count)
    {
        if(random.NextDouble() < 0.6)
        {
            var orderGroups =
                solution.Operations
                    .GroupBy(
                        x => x.OrderCode,
                        StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(x =>
                        x.Max(y =>
                            y.EndSlot))
                    .Take(
                        Math.Max(
                            1,
                            solution.Operations.Count / 9))
                    .ToList();

            if(orderGroups.Count > 0)
            {
                var selectedOrder =
                    orderGroups[
                        random.Next(
                            orderGroups.Count)];

                var removed =
                    selectedOrder
                        .OrderBy(x =>
                            x.JobTicketCode,
                            StringComparer.OrdinalIgnoreCase)
                        .Take(count)
                        .ToList();

                if(removed.Count < count)
                {
                    removed.AddRange(
                        solution.Operations
                            .Except(
                                removed)
                            .OrderByDescending(x =>
                                x.EndSlot)
                            .Take(
                                count -
                                removed.Count));
                }

                return removed;
            }
        }

        return solution.Operations
            .OrderByDescending(x =>
                x.EndSlot)
            .Take(
                Math.Max(
                    1,
                    count / 2))
            .Concat(
                solution.Operations
                    .OrderBy(_ =>
                        random.Next())
                    .Take(
                        count))
            .Distinct()
            .Take(count)
            .ToList();
    }
}
