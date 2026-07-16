using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class SolutionClonerTests
{
    [Fact]
    public void Clone_Should_Create_New_Solution()
    {
        var source =
            new ScheduleState
            {
                Solution =
                    new SchedulingSolution
                    {
                        Operations =
                        [
                            new ScheduledOperation
                            {
                                JobTicketCode = "J001",
                                MachineCode = "M001",
                                StartSlot = 10,
                                DurationSlots = 5
                            }
                        ]
                    },

                Timeline =
                    new TimelineContext(
                        new SchedulingTimeline())
            };


        var clone =
            new SolutionCloner()
                .Clone(source);


        clone.Solution
            .Operations[0]
            .StartSlot = 100;


        Assert.Equal(
            10,
            source.Solution
                .Operations[0]
                .StartSlot);
    }
}