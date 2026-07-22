namespace ProductionScheduling.Algorithm.Configuration;

public sealed class GeneticAlgorithmOptions
{
    /// <summary>
    /// 种群数量
    /// </summary>
    public int PopulationSize { get; set; }
        = 32;

    /// <summary>
    /// 最大迭代代数
    /// </summary>
    public int Generations { get; set; }
        = 120;

    /// <summary>
    /// 精英保留数量
    /// </summary>
    public int EliteCount { get; set; }
        = 3;

    /// <summary>
    /// 交叉概率
    /// </summary>
    public double CrossoverRate { get; set; }
        = 0.8;

    /// <summary>
    /// 变异概率
    /// </summary>
    public double MutationRate { get; set; }
        = 0.2;

    /// <summary>
    /// 锦标赛选择数量
    /// </summary>
    public int TournamentSize { get; set; }
        = 3;

    /// <summary>
    /// 连续多少代无改善后终止
    /// </summary>
    public int MaxNoImprovementGenerations { get; set; }
        = 30;
}
