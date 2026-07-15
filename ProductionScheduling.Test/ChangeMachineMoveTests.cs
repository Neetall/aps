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

public class ChangeMachineMoveTests
{
    [Fact]
    public void ChangeMachineMove_Should_Move_To_Faster_Machine()
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


        var ticket =
            new JobTicket
            {
                Code = "JT001",
                Sequence = 1,
                Length = 100
            };


        order.JobTickets.Add(ticket);



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
                },


                new Machine
                {
                    Code = "M002",

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M002",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 100,
                            SetupMinutes = 0
                        }
                    ]
                }
            };



        /*
         * =========================
         * 3. 创建Context
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
         * 5. 创建初始方案
         *
         * 假设Greedy已经排:
         *
         * JT001 -> M001
         *
         * 08:00-10:00
         *
         * =========================
         */

        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode = "JT001",

                MachineCode = "M001",

                StartSlot = 0,

                DurationSlots = 2
            });



        timeline.Machines["M001"]
            .Occupy(
                0,
                2);



        /*
         * =========================
         * 6. 创建索引
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
         * 8. 执行ChangeMachineMove
         * =========================
         */

        var move =
            new ChangeMachineMove(
                new ScheduleDurationCalculator());



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
         * 应该迁移到M002
         */
        Assert.Equal(
            "M002",
            operation.MachineCode);



        /*
         * M002产能100/h
         *
         * 100长度
         *
         * =1小时
         */
        Assert.Equal(
            1,
            operation.DurationSlots);



        /*
         * 新设备占用
         */
        Assert.True(
            timeline.Machines["M002"]
                .IsFree(0)
                == false);



        /*
         * 老设备释放
         */
        Assert.True(
            timeline.Machines["M001"]
                .IsFree(0));
    }
}