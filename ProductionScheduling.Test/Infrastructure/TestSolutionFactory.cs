using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestSolutionFactory
{
    /// <summary>
    /// 创建一个故意较差的初始方案
    ///
    /// JT001 -> M001
    ///
    /// 用于:
    /// LocalSearch
    /// SA
    /// Move测试
    /// </summary>
    public static SchedulingSolution CreateSlowMachineSolution(
        TimelineContextGroup timelines)
    {
        var solution =
            new SchedulingSolution();



        solution.Operations.Add(
            new ScheduledOperation
            {
                FactoryCode =
                    "F001",

                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                StartSlot =
                    0,

                DurationSlots =
                    2
            });



        var factory =
            timelines.Get(
                "F001");



        factory.Machines["M001"]
            .Occupy(
                0,
                2);



        return solution;
    }
}