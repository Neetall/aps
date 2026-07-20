using Microsoft.Extensions.DependencyInjection;

using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Optimization.Tabu;


namespace ProductionScheduling.Algorithm.Optimization.Factory;

/// <summary>
/// 优化器工厂
///
/// 根据算法类型从DI容器获取优化器
/// </summary>
public class OptimizerFactory
{
    private readonly IServiceProvider provider;


    public OptimizerFactory(
        IServiceProvider provider)
    {
        this.provider =
            provider;
    }



    public ISolutionOptimizer Create(
        OptimizationAlgorithmType type)
    {
        return type switch
        {
            OptimizationAlgorithmType.LocalSearch =>
                provider.GetRequiredService<
                    LocalSearchOptimizer>(),


            OptimizationAlgorithmType.SimulatedAnnealing =>
                provider.GetRequiredService<
                    SimulatedAnnealingOptimizer>(),


            OptimizationAlgorithmType.Tabu =>
                provider.GetRequiredService<
                    TabuSearchOptimizer>(),


            OptimizationAlgorithmType.Lns =>
                provider.GetRequiredService<
                    LnsOptimizer>(),


            OptimizationAlgorithmType.Genetic =>
                throw new NotImplementedException(
                    "Genetic优化器尚未实现"),


            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(type))
        };
    }
}