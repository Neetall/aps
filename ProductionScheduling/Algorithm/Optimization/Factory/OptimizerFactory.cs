using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Validation;

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

    private readonly SchedulingSolutionValidator validator;

    private readonly IDestroyOperator destroyOperator;

    private readonly IRepairOperator repairOperator;

    private readonly ILnsAcceptance lnsAcceptance;

    private readonly MoveNeighborhoodGenerator neighborhoodGenerator;

    private readonly SchedulingAlgorithmOptions options;


    public OptimizerFactory(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelectorFactory moveSelectorFactory,
        SolutionCloner cloner,
        SchedulingSolutionValidator validator,
        IDestroyOperator destroyOperator,
        IRepairOperator repairOperator,
        ILnsAcceptance lnsAcceptance,
        MoveNeighborhoodGenerator neighborhoodGenerator,
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

        this.validator =
            validator;

        this.destroyOperator =
            destroyOperator;

        this.repairOperator =
            repairOperator;

        this.lnsAcceptance =
            lnsAcceptance;

        this.neighborhoodGenerator =
            neighborhoodGenerator;

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
            resourceIndex,
            jobTicketIndex,
            operationSelector,
            moveSelectorFactory.Create(),
            cloner,
            validator,
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
            validator,
            options.SimulatedAnnealing);
    }



    private ISolutionOptimizer CreateTabu()
    {
        return new TabuSearchOptimizer(
            resourceIndex,
            jobTicketIndex,
            neighborhoodGenerator,
            cloner,
            validator,
            options.TabuSearch);
    }



    private ISolutionOptimizer CreateLns()
    {
        return new LnsOptimizer(
            cloner,
            destroyOperator,
            repairOperator,
            lnsAcceptance,
            validator,
            options.Lns);
    }
}