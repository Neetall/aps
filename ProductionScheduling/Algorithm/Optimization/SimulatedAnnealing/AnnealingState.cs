namespace ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;

/// <summary>
///     模拟退火状态
/// </summary>
public class AnnealingState
{
    /// <summary>
    ///     当前温度
    /// </summary>
    public double Temperature { get; set; }


    /// <summary>
    ///     当前迭代次数
    /// </summary>
    public int Iteration { get; set; }


    /// <summary>
    ///     当前评分
    /// </summary>
    public double CurrentScore { get; set; }


    /// <summary>
    ///     最优评分
    /// </summary>
    public double BestScore { get; set; }
}