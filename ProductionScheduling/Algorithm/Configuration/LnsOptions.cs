namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// Large Neighborhood Search参数
/// </summary>
public sealed class LnsOptions
{
    /// <summary>
    /// 最大迭代次数
    /// </summary>
    public int Iterations { get; init; } = 1000;


    /// <summary>
    /// 每次破坏比例
    /// </summary>
    public double DestroyRate { get; init; } = 0.2;
}