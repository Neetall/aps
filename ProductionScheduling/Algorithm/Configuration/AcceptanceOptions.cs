namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// SA接受策略配置
/// </summary>
public sealed class AcceptanceOptions
{
    /// <summary>
    /// 最大接受概率
    ///
    /// 防止高温阶段随机性过强
    /// </summary>
    public double MaximumProbability { get; init; } = 1.0;


    /// <summary>
    /// 能量差缩放比例
    ///
    /// 用于调整Score数量级
    /// </summary>
    public double ScoreScale { get; init; } = 1.0;
}