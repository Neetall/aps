using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Algorithm.Moves.Implementations;

namespace ProductionScheduling.Algorithm.Optimization.Selection;

public class MoveSelectorFactory
{
    private readonly MoveOptions options;

    private readonly ScheduleDurationCalculator durationCalculator;

    private readonly AlgorithmDebugOptions debugOptions;


    public MoveSelectorFactory(
        MoveOptions options,
        ScheduleDurationCalculator durationCalculator,
        AlgorithmDebugOptions debugOptions)
    {
        this.options =
            options;

        this.durationCalculator =
            durationCalculator;

        this.debugOptions =
            debugOptions;
    }



    public MoveSelector Create()
    {
        var selector =
            new MoveSelector();



        selector.Register(
            new ChangeMachineMove(
                durationCalculator,
                debugOptions),
            options.ChangeMachineWeight);



        selector.Register(
            new ShiftTimeMove(
                debugOptions),
            options.ShiftTimeWeight);



        selector.Register(
            new SwapOperationMove(
                debugOptions),
            options.SwapOperationWeight);



        return selector;
    }
}