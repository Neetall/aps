using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Algorithm.Optimization;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class ShiftTimeMoveTests
{
    [Fact]
    public void ShiftTimeMove_Should_Move_Operation_To_New_Time()
    {
        /*
         * =========================
         * 1. 创建工单
         * =========================
         */

        var order =
            new Order
            {
                Code = "ORD001",
                Priority = 1
            };


        order.JobTickets.Add(
            new JobTicket
            {
                Code = "JT001",

                Sequence = 1,

                Length = 100
            });



        /*
         * =========================
         * 2. 创建设备
         * =========================
         */

        var machines =
            new List<Machine>
            {
                new Machine
                {
                    Code = "M001",

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M001",

                            JobTicketCode = "JT001",

                            HourlyCapacity = 50,

                            SetupMinutes = 0
                        }
                    ]
                }
            };



        /*
         * =========================
         * 3. 创建SchedulingContext
         * =========================
         */

        var context =
            new SchedulingContext
            {
                Orders =
                [
                    order
                ],

                Machines =
                    machines,

                Options =
                {
                    TimeGranularityMinutes = 60
                }
            };


        context.FactoryCalendars.Add(
            new FactoryCalendar
            {
                Periods =
                [
                    new ShiftPeriod
                    {
                        StartTime =
                            DateTime.Today
                                .AddHours(8),

                        EndTime =
                            DateTime.Today
                                .AddHours(18)
                    }
                ]
            });



        /*
         * =========================
         * 4. 初始化Timeline
         * =========================
         */

        var timeline =
            new TimelineBuilder()
                .Build(context);



        /*
         * =========================
         * 5. 创建初始排产
         *
         * JT001:
         *
         * 08-10
         *
         * Duration=2
         *
         * =========================
         */

        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
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



        /*
         * =========================
         * 6. 创建资源索引
         * =========================
         */

        var resourceIndex =
            new SchedulingResourceIndex();


        resourceIndex.Build(
            machines);



        /*
         * =========================
         * 7. 创建MoveContext
         * =========================
         */

        var moveContext =
            new MoveContext
            {
                SchedulingContext =
                    context,

                Solution =
                    solution,

                Timeline =
                    timeline,

                ResourceIndex =
                    resourceIndex
            };



        /*
         * =========================
         * 8. 执行ShiftTimeMove
         * =========================
         */

        var move =
            new ShiftTimeMove();



        var result =
            move.Apply(
                moveContext);



        /*
         * =========================
         * 9. 验证
         * =========================
         */

        Assert.True(result);



        var operation =
            solution.Operations[0];



        /*
         * 工单仍然是JT001
         */
        Assert.Equal(
            "JT001",
            operation.JobTicketCode);



        /*
         * 设备没有变化
         */
        Assert.Equal(
            "M001",
            operation.MachineCode);



        /*
         * 时间发生变化
         */
        Assert.NotEqual(
            0,
            operation.StartSlot);



        /*
         * 新时间被占用
         */
        Assert.False(
            timeline.Machines["M001"]
                .IsFree(
                    operation.StartSlot));



        /*
         * 原时间释放
         */
        Assert.True(
            timeline.Machines["M001"]
                .IsFree(0));
    }
}