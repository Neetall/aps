using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Scheduling;

namespace ProductionScheduling.Algorithm.Optimization.Selection;

public class MoveNeighborhoodGenerator
{
    private readonly MoveSelector moveSelector;


    public MoveNeighborhoodGenerator(
        MoveSelector moveSelector)
    {
        this.moveSelector =
            moveSelector;
    }


    public List<MoveCandidate> Generate(
        SchedulingSolution solution)
    {
        var result =
            new List<MoveCandidate>();


        var moves =
            moveSelector.GetAll();


        foreach (var operation in solution.Operations)
        foreach (var move in moves)
            result.Add(
                new MoveCandidate
                {
                    Operation =
                        operation,

                    Move =
                        move
                });


        return result;
    }
}