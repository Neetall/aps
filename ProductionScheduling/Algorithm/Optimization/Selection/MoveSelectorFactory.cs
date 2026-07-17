using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Algorithm.Moves.Implementations;

namespace ProductionScheduling.Algorithm.Optimization.Selection;

public class MoveSelectorFactory
{
    private readonly MoveOptions options;


    public MoveSelectorFactory(
        MoveOptions options)
    {
        this.options =
            options;
    }


    public MoveSelector Create()
    {
        var selector =
            new MoveSelector();


        selector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()),
            options.ChangeMachineWeight);


        selector.Register(
            new ShiftTimeMove(),
            options.ShiftTimeWeight);


        selector.Register(
            new SwapOperationMove(),
            options.SwapOperationWeight);


        return selector;
    }
}