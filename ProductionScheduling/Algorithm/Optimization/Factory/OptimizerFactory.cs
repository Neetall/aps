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
/// 根据配置创建具体优化算法
/// </summary>
public class OptimizerFactory
{
    private readonly OptimizerFactoryContext context;


    public OptimizerFactory(
        OptimizerFactoryContext context)
    {
        this.context =
            context;
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
                CreateTabu(),


            OptimizationAlgorithmType.Lns =>
                CreateLns(),


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
            context.ResourceIndex,
            context.JobTicketIndex,
            context.OperationSelector,
            context.MoveSelectorFactory.Create(),
            context.Cloner,
            context.Validator,
            context.Options.LocalSearch);
    }



    private ISolutionOptimizer CreateSimulatedAnnealing()
    {
        return new SimulatedAnnealingOptimizer(
            context.ResourceIndex,
            context.JobTicketIndex,
            context.OperationSelector,
            context.MoveSelectorFactory.Create(),
            context.Cloner,
            new AcceptanceCriteria(
                context.Options.Acceptance),
            context.Validator,
            context.Options.SimulatedAnnealing);
    }



    private ISolutionOptimizer CreateTabu()
    {
        return new TabuSearchOptimizer(
            context.ResourceIndex,
            context.JobTicketIndex,
            context.NeighborhoodGenerator,
            context.Cloner,
            context.Validator,
            context.Options.TabuSearch);
    }



    private ISolutionOptimizer CreateLns()
    {
        return new LnsOptimizer(
            context.Cloner,
            context.DestroyOperator,
            context.RepairOperator,
            context.LnsAcceptance,
            context.Validator,
            context.Options.Lns);
    }
}