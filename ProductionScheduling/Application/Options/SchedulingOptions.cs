namespace ProductionScheduling.Application.Options;

/// <summary>
///     排产算法配置
/// </summary>
public class SchedulingOptions
{
    /// <summary>
    ///     调度算法类型
    /// </summary>
    public SchedulingAlgorithmType AlgorithmType { get; set; } = SchedulingAlgorithmType.HybridGA;


    /// <summary>
    ///     时间粒度（分钟）
    ///     默认60分钟，即小时级排产
    /// </summary>
    public int TimeGranularityMinutes { get; set; } = 60;


    /// <summary>
    ///     最大计算时间（秒）
    ///     防止算法无限运行
    /// </summary>
    public int MaxExecutionSeconds { get; set; } = 60;


    /// <summary>
    ///     是否允许超过交期
    ///     false表示交期为硬约束
    /// </summary>
    public bool AllowDueDateOverrun { get; set; } = false;


    /// <summary>
    ///     是否优先保证最早完工
    /// </summary>
    public bool MinimizeMakespan { get; set; } = true;


    /// <summary>
    ///     是否优化设备利用率
    /// </summary>
    public bool OptimizeMachineUtilization { get; set; } = true;


    /// <summary>
    ///     是否输出算法过程日志
    /// </summary>
    public bool EnableTrace { get; set; } = false;


    #region Genetic Algorithm

    /// <summary>
    ///     种群数量
    /// </summary>
    public int PopulationSize { get; set; } = 100;


    /// <summary>
    ///     最大迭代代数
    /// </summary>
    public int MaxGenerations { get; set; } = 200;


    /// <summary>
    ///     交叉概率
    /// </summary>
    public double CrossoverRate { get; set; } = 0.8;


    /// <summary>
    ///     变异概率
    /// </summary>
    public double MutationRate { get; set; } = 0.05;

    #endregion


    #region Simulated Annealing

    /// <summary>
    ///     初始温度
    /// </summary>
    public double InitialTemperature { get; set; } = 1000;


    /// <summary>
    ///     冷却系数
    /// </summary>
    public double CoolingRate { get; set; } = 0.95;


    /// <summary>
    ///     每轮邻域搜索次数
    /// </summary>
    public int NeighborhoodIterations { get; set; } = 100;

    #endregion


    #region CP-SAT

    /// <summary>
    ///     CP-SAT搜索时间限制
    /// </summary>
    public int CpSatTimeLimitSeconds { get; set; } = 60;


    /// <summary>
    ///     CP-SAT线程数
    /// </summary>
    public int CpSatWorkers { get; set; } = Environment.ProcessorCount;

    #endregion
}

/// <summary>
///     排产算法类型
/// </summary>
public enum SchedulingAlgorithmType
{
    /// <summary>
    ///     Google OR-Tools CP-SAT
    /// </summary>
    CpSat = 1,


    /// <summary>
    ///     自研混合遗传退火算法
    /// </summary>
    HybridGA = 2,


    /// <summary>
    ///     仅使用主动调度
    /// </summary>
    ActiveScheduler = 3,


    /// <summary>
    ///     禁忌搜索
    /// </summary>
    TabuSearch = 4
}