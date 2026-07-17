using ProductionScheduling.Algorithm.Optimization.Tabu;
using Xunit;

namespace ProductionScheduling.Test;

public class TabuMemoryTests
{
    [Fact]
    public void TabuMemory_Should_Add_And_Find_Tabu_Entry()
    {
        /*
         * Arrange
         */

        var memory =
            new TabuMemory(
                5);


        memory.Add(
            "ChangeMachine:JT001:M001->M002",
            1);


        /*
         * Act
         */

        var result =
            memory.IsTabu(
                "ChangeMachine:JT001:M001->M002",
                2);


        /*
         * Assert
         */

        Assert.True(
            result);


        Assert.Equal(
            1,
            memory.Count);
    }


    [Fact]
    public void TabuMemory_Should_Expire_Entry()
    {
        /*
         * Arrange
         */

        var memory =
            new TabuMemory(
                3);


        memory.Add(
            "ShiftTime:JT001:0->5",
            1);


        /*
         * iteration:
         *
         * 1 + tenure(3)
         *
         * = 4
         *
         * 到4失效
         */

        var result =
            memory.IsTabu(
                "ShiftTime:JT001:0->5",
                4);


        /*
         * Assert
         */

        Assert.False(
            result);


        Assert.Equal(
            0,
            memory.Count);
    }


    [Fact]
    public void TabuMemory_Should_Not_Match_Other_Key()
    {
        /*
         * Arrange
         */

        var memory =
            new TabuMemory(
                10);


        memory.Add(
            "ChangeMachine:JT001:M001->M002",
            1);


        /*
         * Act
         */

        var result =
            memory.IsTabu(
                "ChangeMachine:JT001:M001->M003",
                2);


        /*
         * Assert
         */

        Assert.False(
            result);
    }
}