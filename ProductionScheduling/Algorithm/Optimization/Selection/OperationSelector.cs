using ProductionScheduling.Algorithm.Scheduling;

namespace ProductionScheduling.Algorithm.Optimization.Selection;

/// <summary>
///     排产操作选择器
///     从当前方案中选择一个需要优化的派工操作
/// </summary>
public class OperationSelector
{
    private readonly Random random;


    public OperationSelector(Random? random = null)
    {
        this.random = random ?? new Random();
    }


    /// <summary>
    ///     随机选择一个操作
    /// </summary>
    public ScheduledOperation? Select(
        SchedulingSolution solution)
    {
        if (solution.Operations.Count == 0) return null;
        var index = random.Next(solution.Operations.Count);
        return solution.Operations[index];
    }
}