using ProductionScheduling.Algorithm.Configuration;

namespace ProductionScheduling.Domain.Scheduling;

public class SchedulingExecutionOptions
{
    /// <summary>
    /// 是否执行优化
    /// </summary>
    public bool EnableOptimization { get; set; }


    /// <summary>
    /// 使用的优化算法
    /// </summary>
    public OptimizationAlgorithmType AlgorithmType { get; set; }
        = OptimizationAlgorithmType.LocalSearch;
}