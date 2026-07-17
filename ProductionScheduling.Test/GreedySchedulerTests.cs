using ProductionScheduling.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace ProductionScheduling.Test;

public class GreedySchedulerTests
{
    private readonly ITestOutputHelper output;


    public GreedySchedulerTests(
        ITestOutputHelper output)
    {
        this.output =
            output;
    }


    [Fact]
    public void GreedyScheduler_Should_Create_Valid_Schedule()
    {
        /*
         * Arrange
         */

        var context =
            TestSchedulingDataFactory
                .CreateGreedySchedulerContext();


        var scheduler =
            TestAlgorithmFactory
                .CreateGreedyScheduler(
                    context);


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        /*
         * Act
         */

        var solution =
            scheduler.Schedule(
                context,
                timelines);



        /*
         * 输出排产结果
         */

        output.WriteLine(
            "========== 排产结果 ==========");



        foreach(var operation in solution.Operations)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);



            var start =
                factory.TimeModel
                    .GetSlot(
                        operation.StartSlot)
                    .StartTime;



            var end =
                factory.TimeModel
                    .GetSlot(
                        operation.StartSlot +
                        operation.DurationSlots -
                        1)
                    .EndTime;



            output.WriteLine(
                $"工单:{operation.JobTicketCode} | " +
                $"设备:{operation.MachineCode} | " +
                $"开始:{start:yyyy-MM-dd HH:mm} | " +
                $"结束:{end:yyyy-MM-dd HH:mm} | " +
                $"持续:{operation.DurationSlots}小时");
        }



        output.WriteLine(
            "============================");



        /*
         * Assert
         */

        Assert.True(
            solution.IsFeasible);



        Assert.Equal(
            2,
            solution.Operations.Count);



        var first =
            solution.Operations[0];


        var second =
            solution.Operations[1];



        Assert.Equal(
            "M002",
            first.MachineCode);



        Assert.True(
            second.StartSlot >=
            first.StartSlot +
            first.DurationSlots);



        Assert.False(
            first.StartSlot <
            second.StartSlot +
            second.DurationSlots
            &&
            second.StartSlot <
            first.StartSlot +
            first.DurationSlots);
    }
}