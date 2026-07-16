namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 优化算法类型
///
/// 用于定义优化流水线中的算法节点
/// </summary>
public enum OptimizationAlgorithmType
{
    /// <summary>
    /// 局部搜索
    ///
    /// 快速下降，改善当前解
    /// </summary>
    LocalSearch,


    /// <summary>
    /// 模拟退火
    ///
    /// 接受一定概率差解
    /// 跳出局部最优
    /// </summary>
    SimulatedAnnealing,


    /// <summary>
    /// 禁忌搜索
    ///
    /// 使用历史记忆避免循环
    /// </summary>
    Tabu,


    /// <summary>
    /// 大邻域搜索
    ///
    /// 破坏-修复策略
    /// </summary>
    Lns,


    /// <summary>
    /// 遗传算法
    ///
    /// 群体搜索
    /// </summary>
    Genetic
}