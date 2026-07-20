namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 排产算法总配置
///
/// 所有算法参数入口
/// </summary>
public sealed class SchedulingAlgorithmOptions
{
    /// <summary>
    /// 优化算法执行流水线
    ///
    /// 按Order顺序执行
    /// </summary>
    public List<OptimizationStepOptions> Pipeline { get; set; }
        =
        [
            new()
            {
                Enabled = true,
                Order = 1,
                Algorithm =
                    OptimizationAlgorithmType.LocalSearch
            },

            new()
            {
                Enabled = true,
                Order = 2,
                Algorithm =
                    OptimizationAlgorithmType.SimulatedAnnealing
            },

            new()
            {
                Enabled = true,
                Order = 3,
                Algorithm =
                    OptimizationAlgorithmType.Tabu
            },

            new()
            {
                Enabled = true,
                Order = 4,
                Algorithm =
                    OptimizationAlgorithmType.Lns
            }
        ];


    /// <summary>
    /// 局部搜索配置
    /// </summary>
    public LocalSearchOptions LocalSearch { get; init; }
        = new();


    /// <summary>
    /// 模拟退火配置
    /// </summary>
    public SimulatedAnnealingOptions SimulatedAnnealing { get; init; }
        = new();


    /// <summary>
    /// 接受准则配置
    /// </summary>
    public AcceptanceOptions Acceptance { get; init; }
        = new();


    /// <summary>
    /// 禁忌搜索配置
    /// </summary>
    public TabuSearchOptions TabuSearch { get; init; }
        = new();


    /// <summary>
    /// LNS配置
    /// </summary>
    public LnsOptions Lns { get; init; }
        = new();


    /// <summary>
    /// Move配置
    /// </summary>
    public MoveOptions Moves { get; init; }
        = new();
}