namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 模拟退火参数
/// </summary>
public sealed class SimulatedAnnealingOptions
{
    /// <summary>
    /// 最大迭代次数
    /// </summary>
    public int Iterations { get; init; } = 1200;


    /// <summary>
    /// 初始温度
    /// </summary>
    public double InitialTemperature { get; init; } = 500;


    /// <summary>
    /// 降温比例
    /// </summary>
    public double CoolingRate { get; init; } = 0.985;


    /// <summary>
    /// 最低温度
    /// </summary>
    public double MinimumTemperature { get; init; } = 0.1;
}
