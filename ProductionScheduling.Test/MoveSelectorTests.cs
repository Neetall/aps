using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using Xunit;

namespace ProductionScheduling.Test;

public class MoveSelectorTests
{
    [Fact]
    public void MoveSelector_Should_Select_Registered_Move()
    {
        var selector =
            new MoveSelector(
                new Random(1));


        var move =
            new FakeMove();


        selector.Register(
            move,
            10);


        var result =
            selector.Select();


        Assert.NotNull(
            result);


        Assert.Same(
            move,
            result);
    }



    [Fact]
    public void MoveSelector_Should_Select_From_Multiple_Moves()
    {
        var selector =
            new MoveSelector(
                new Random(1));


        var move1 =
            new FakeMove();


        var move2 =
            new FakeMove();


        selector.Register(
            move1,
            1);


        selector.Register(
            move2,
            1);



        var result =
            selector.Select();



        Assert.NotNull(
            result);


        Assert.True(
            result == move1 ||
            result == move2);
    }



    [Fact]
    public void MoveSelector_Should_Throw_When_No_Move()
    {
        var selector =
            new MoveSelector(
                new Random(1));


        Assert.Throws<InvalidOperationException>(
            () =>
                selector.Select());
    }



    private class FakeMove : IMove
    {
        public string Name =>
            "FakeMove";


        public bool Apply(
            MoveContext context)
        {
            return true;
        }


        public void Undo(
            MoveContext context)
        {
        }
    }
}