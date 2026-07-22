namespace ProductionScheduling.Algorithm.Configuration;

public sealed class CpSatOptions
{
    /// <summary>
    /// 最大求解时间
    /// 单位:秒
    /// </summary>
    public double MaxSolveSeconds { get; set; }
        = 10;

    /// <summary>
    /// 并行搜索线程数
    /// 0表示由求解器决定
    /// </summary>
    public int WorkerCount { get; set; }
        = 0;

    /// <summary>
    /// 是否输出求解日志
    /// </summary>
    public bool EnableSolverLog { get; set; }

    /// <summary>
    /// 是否使用现有方案作为Hint
    /// </summary>
    public bool UseInitialSolutionHint { get; set; }
        = true;

    /// <summary>
    /// 是否要求证明最优
    ///
    /// false:
    /// 时间截止时允许返回可行解
    /// </summary>
    public bool RequireOptimal { get; set; }

    /// <summary>
    /// 延期订单数量权重
    /// </summary>
    public int DelayCountWeight { get; set; }
        = 100000;

    /// <summary>
    /// 最大完工时间权重
    /// </summary>
    public int MakespanWeight { get; set; }
        = 10000;

    /// <summary>
    /// 延期Slot数量权重
    /// </summary>
    public int TardinessWeight { get; set; }
        = 1000;
}
