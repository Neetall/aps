using ProductionScheduling.Algorithm.Scheduling;
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
        TimelineContext timeline,
        double rate)
    {
        var count =
            Math.Max(
                1,
                (int)(
                    solution.Operations.Count
                    *
                    rate));



        var removed =
            solution.Operations
                .OrderBy(x =>
                    random.Next())
                .Take(count)
                .ToList();



        foreach(var operation in removed)
        {
            if(timeline.Machines
               .TryGetValue(
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