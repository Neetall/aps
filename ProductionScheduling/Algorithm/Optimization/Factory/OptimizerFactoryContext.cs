using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Validation;

namespace ProductionScheduling.Algorithm.Optimization.Factory;

public class OptimizerFactoryContext
{
    public SchedulingResourceIndex ResourceIndex { get; init; }

    public JobTicketIndex JobTicketIndex { get; init; }

    public OperationSelector OperationSelector { get; init; }

    public MoveSelectorFactory MoveSelectorFactory { get; init; }

    public SolutionCloner Cloner { get; init; }

    public SchedulingSolutionValidator Validator { get; init; }


    public IDestroyOperator DestroyOperator { get; init; }

    public IRepairOperator RepairOperator { get; init; }

    public ILnsAcceptance LnsAcceptance { get; init; }


    public MoveNeighborhoodGenerator NeighborhoodGenerator { get; init; }


    public SchedulingAlgorithmOptions Options { get; init; }
}