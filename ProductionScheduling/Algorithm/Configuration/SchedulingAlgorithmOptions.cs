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
                Enabled = false,
                Order = 1,
                Algorithm =
                    OptimizationAlgorithmType.LocalSearch
            },

            new()
            {
                Enabled = false,
                Order = 2,
                Algorithm =
                    OptimizationAlgorithmType.SimulatedAnnealing
            },

            new()
            {
                Enabled = false,
                Order = 3,
                Algorithm =
                    OptimizationAlgorithmType.Tabu
            },

            new()
            {
                Enabled = false,
                Order = 4,
                Algorithm =
                    OptimizationAlgorithmType.Lns
            },
            new()
            {
                Enabled = false,
                Order = 5,
                Algorithm =
                    OptimizationAlgorithmType.GeneticAlgorithm
            },
            new()
            {
                Enabled = true,
                Order = 6,
                Algorithm =
                    OptimizationAlgorithmType.CpSat
            }
        ];


    /// <summary>
    /// 局部搜索配置
    /// </summary>
    public LocalSearchOptions LocalSearch { get; set; }
        = new();


    /// <summary>
    /// 模拟退火配置
    /// </summary>
    public SimulatedAnnealingOptions SimulatedAnnealing { get; set; }
        = new();


    /// <summary>
    /// 接受准则配置
    /// </summary>
    public AcceptanceOptions Acceptance { get; set; }
        = new();


    /// <summary>
    /// 禁忌搜索配置
    /// </summary>
    public TabuSearchOptions TabuSearch { get; set; }
        = new();


    /// <summary>
    /// LNS配置
    /// </summary>
    public LnsOptions Lns { get; set; }
        = new();


    /// <summary>
    /// Move配置
    /// </summary>
    public MoveOptions Moves { get; set; }
        = new();
    
    public GeneticAlgorithmOptions GeneticAlgorithm { get; set; }
        = new();

    public CpSatOptions CpSat { get; set; }
        = new();

    public EvaluationScoreOptions Evaluation { get; set; }
        = new();

    public OptimizationEffectivenessOptions Effectiveness { get; set; }
        = new();
}
