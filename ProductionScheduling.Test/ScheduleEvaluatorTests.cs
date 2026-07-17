using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class ScheduleEvaluatorTests
{
    [Fact]
    public void Faster_Machine_Should_Have_Better_Score()
    {
        /*
         * Arrange
         */

        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timeline =
            TestTimelineFactory
                .Create(
                    context);


        var evaluator =
            new ScheduleEvaluator();


        /*
         * 慢设备方案
         *
         * JT001 -> M001
         */
        var slowSolution =
            new SchedulingSolution();


        slowSolution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                StartSlot =
                    0,

                DurationSlots =
                    2
            });


        timeline.Machines["M001"]
            .Occupy(
                0,
                2);


        var slowResult =
            evaluator.Evaluate(
                slowSolution,
                timeline,
                context);


        /*
         * 创建新的Timeline
         *
         * 防止污染
         */
        var fastTimeline =
            TestTimelineFactory
                .Create(
                    context);


        /*
         * 快设备方案
         *
         * JT001 -> M002
         */
        var fastSolution =
            new SchedulingSolution();


        fastSolution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M002",

                StartSlot =
                    0,

                DurationSlots =
                    1
            });


        fastTimeline.Machines["M002"]
            .Occupy(
                0,
                1);


        var fastResult =
            evaluator.Evaluate(
                fastSolution,
                fastTimeline,
                context);


        /*
         * Assert
         */

        Assert.True(
            fastResult.Score <
            slowResult.Score);
    }
}