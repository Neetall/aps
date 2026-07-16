namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 优化流程节点配置
///
/// 描述一次优化阶段
/// </summary>
public sealed class OptimizationStepOptions
{
    /// <summary>
    /// 使用的优化算法
    /// </summary>
    public OptimizationAlgorithmType Algorithm { get; init; }



    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; init; } = true;



    /// <summary>
    /// 执行顺序
    /// 数字越小越优先
    /// </summary>
    public int Order { get; init; }
}