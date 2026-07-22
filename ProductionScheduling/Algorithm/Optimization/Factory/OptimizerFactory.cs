using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.CpSat;
using ProductionScheduling.Algorithm.Optimization.Genetic;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Optimization.Tabu;

namespace ProductionScheduling.Algorithm.Optimization.Factory;

/// <summary>
/// 优化器工厂
/// 根据算法类型返回对应优化器
/// </summary>
public sealed class OptimizerFactory
{
    private readonly LocalSearchOptimizer localSearch;
    private readonly SimulatedAnnealingOptimizer simulatedAnnealing;
    private readonly TabuSearchOptimizer tabu;
    private readonly LnsOptimizer lns;
    private readonly GeneticAlgorithmOptimizer geneticAlgorithm;
    private readonly CpSatOptimizer cpSat;

    public OptimizerFactory(
        LocalSearchOptimizer localSearch,
        SimulatedAnnealingOptimizer simulatedAnnealing,
        TabuSearchOptimizer tabu,
        LnsOptimizer lns,
        GeneticAlgorithmOptimizer geneticAlgorithm,
        CpSatOptimizer cpSat)
    {
        this.localSearch = localSearch;
        this.simulatedAnnealing = simulatedAnnealing;
        this.tabu = tabu;
        this.lns = lns;
        this.geneticAlgorithm = geneticAlgorithm;
        this.cpSat = cpSat;
    }

    public ISolutionOptimizer Create(
        OptimizationAlgorithmType type)
    {
        return type switch
        {
            OptimizationAlgorithmType.LocalSearch =>
                localSearch,

            OptimizationAlgorithmType.SimulatedAnnealing =>
                simulatedAnnealing,

            OptimizationAlgorithmType.Tabu =>
                tabu,

            OptimizationAlgorithmType.Lns =>
                lns,

            OptimizationAlgorithmType.GeneticAlgorithm =>
                geneticAlgorithm,

            OptimizationAlgorithmType.CpSat =>
                cpSat,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(type),
                    type,
                    $"不支持的优化算法:{type}")
        };
    }
}