using ProductionScheduling.Algorithm.Moves.Core;
using Xunit;

namespace ProductionScheduling.Test;

public class MoveExecutionRecordTests
{
    [Fact]
    public void MoveExecutionRecord_Should_Store_Move_Information()
    {
        var record =
            new MoveExecutionRecord
            {
                MoveName =
                    "ChangeMachine",

                Success =
                    true,

                JobTicketCode =
                    "JT001",

                SecondJobTicketCode =
                    "JT002",

                OldMachineCode =
                    "M001",

                SecondOldMachineCode =
                    "M002",

                OldStartSlot =
                    10,

                NewStartSlot =
                    20,

                SecondOldStartSlot =
                    30,

                SecondNewStartSlot =
                    40,

                OldDurationSlots =
                    5,

                NewDurationSlots =
                    3,

                SecondDurationSlots =
                    4,

                OldScore =
                    100,

                NewScore =
                    80,

                Accepted =
                    true
            };


        Assert.Equal(
            "ChangeMachine",
            record.MoveName);


        Assert.True(
            record.Success);


        Assert.Equal(
            "JT001",
            record.JobTicketCode);


        Assert.Equal(
            "JT002",
            record.SecondJobTicketCode);


        Assert.Equal(
            "M001",
            record.OldMachineCode);


        Assert.Equal(
            "M002",
            record.SecondOldMachineCode);


        Assert.Equal(
            10,
            record.OldStartSlot);


        Assert.Equal(
            20,
            record.NewStartSlot);


        Assert.Equal(
            100,
            record.OldScore);


        Assert.Equal(
            80,
            record.NewScore);


        Assert.True(
            record.Accepted);
    }



    [Fact]
    public void MoveExecutionRecord_Should_Default_State()
    {
        var record =
            new MoveExecutionRecord();


        Assert.False(
            record.Success);


        Assert.False(
            record.Accepted);


        Assert.Equal(
            0,
            record.OldStartSlot);


        Assert.Equal(
            0,
            record.NewStartSlot);
    }
}