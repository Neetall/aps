namespace ProductionScheduling.Algorithm.Scheduling;

/// <summary>
/// 排产优化算法类型
/// </summary>
public enum SchedulingAlgorithmType
{
    /// <summary>
    /// 贪心算法
    /// </summary>
    Greedy,


    /// <summary>
    /// 局部搜索
    /// </summary>
    LocalSearch,


    /// <summary>
    /// 模拟退火
    /// </summary>
    SimulatedAnnealing,


    /// <summary>
    /// 禁忌搜索
    /// </summary>
    Tabu,


    /// <summary>
    /// 大邻域搜索
    /// </summary>
    Lns
}