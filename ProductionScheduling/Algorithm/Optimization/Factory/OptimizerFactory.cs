using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;

namespace ProductionScheduling.Algorithm.Optimization.Factory;

/// <summary>
/// 优化器工厂
///
/// 根据配置创建具体优化算法
/// </summary>
public class OptimizerFactory
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly OperationSelector operationSelector;

    private readonly MoveSelectorFactory moveSelectorFactory;

    private readonly SolutionCloner cloner;

    private readonly SchedulingAlgorithmOptions options;


    public OptimizerFactory(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelectorFactory moveSelectorFactory,
        SolutionCloner cloner,
        SchedulingAlgorithmOptions options)
    {
        this.resourceIndex =
            resourceIndex;

        this.jobTicketIndex =
            jobTicketIndex;

        this.operationSelector =
            operationSelector;

        this.moveSelectorFactory =
            moveSelectorFactory;

        this.cloner =
            cloner;

        this.options =
            options;
    }


    /// <summary>
    /// 创建优化器
    /// </summary>
    public ISolutionOptimizer Create(
        OptimizationAlgorithmType type)
    {
        return type switch
        {
            OptimizationAlgorithmType.LocalSearch =>
                CreateLocalSearch(),


            OptimizationAlgorithmType.SimulatedAnnealing =>
                CreateSimulatedAnnealing(),


            OptimizationAlgorithmType.Tabu =>
                throw new NotImplementedException(
                    "Tabu优化器尚未实现"),


            OptimizationAlgorithmType.Lns =>
                throw new NotImplementedException(
                    "LNS优化器尚未实现"),


            OptimizationAlgorithmType.Genetic =>
                throw new NotImplementedException(
                    "Genetic优化器尚未实现"),


            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(type))
        };
    }


    private ISolutionOptimizer CreateLocalSearch()
    {
        return new LocalSearchOptimizer(
            resourceIndex,
            jobTicketIndex,
            operationSelector,
            moveSelectorFactory.Create(),
            cloner,
            options.LocalSearch);
    }


    private ISolutionOptimizer CreateSimulatedAnnealing()
    {
        return new SimulatedAnnealingOptimizer(
            resourceIndex,
            jobTicketIndex,
            operationSelector,
            moveSelectorFactory.Create(),
            cloner,
            new AcceptanceCriteria(
                options.Acceptance),
            options.SimulatedAnnealing);
    }
}