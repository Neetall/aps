namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 排产算法总配置
///
/// 所有算法参数入口
/// </summary>
public class SchedulingAlgorithmOptions
{
    /// <summary>
    /// 局部搜索配置
    /// </summary>
    public LocalSearchOptions LocalSearch { get; init; } = new();



    /// <summary>
    /// 模拟退火配置
    /// </summary>
    public SimulatedAnnealingOptions SimulatedAnnealing { get; init; } = new();



    /// <summary>
    /// 禁忌搜索配置
    /// </summary>
    public TabuSearchOptions TabuSearch { get; init; } = new();


    public AcceptanceOptions Acceptance { get; }

    /// <summary>
    /// LNS配置
    /// </summary>
    public LnsOptions Lns { get; init; } = new();



    /// <summary>
    /// Move配置
    /// </summary>
    public MoveOptions Moves { get; init; } = new();
    
    public SchedulingAlgorithmOptions()
    {
        LocalSearch = new LocalSearchOptions();
        SimulatedAnnealing = new SimulatedAnnealingOptions();
        Lns = new LnsOptions();
        Moves = new MoveOptions();
        Acceptance = new AcceptanceOptions();
    }
}