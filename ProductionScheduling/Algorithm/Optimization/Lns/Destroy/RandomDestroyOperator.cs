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
            solution.Operations
                .OrderBy(_ =>
                    random.Next())
                .Take(count)
                .ToList();



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
}