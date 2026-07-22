using ProductionScheduling.Algorithm.Configuration;

namespace ProductionScheduling.Domain.Scheduling;

public class SchedulingExecutionOptions
{
    /// <summary>
    /// 是否执行优化流程
    ///
    /// false:
    /// 只返回Greedy结果
    ///
    /// true:
    /// 执行完整优化Pipeline
    /// </summary>
    public bool EnableOptimization { get; set; }



    /// <summary>
    /// 优化Pipeline配置
    ///
    /// 按顺序执行:
    /// LocalSearch
    /// SA
    /// Tabu
    /// LNS
    /// </summary>
    public List<OptimizationAlgorithmType> Algorithms { get; set; }
        = [];



    /// <summary>
    /// 优化总超时时间，单位秒。
    ///
    /// 0表示不限制。
    /// </summary>
    public int TimeoutSeconds { get; set; }
}
