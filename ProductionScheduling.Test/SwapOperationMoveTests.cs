using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class SwapOperationMoveTests
{
    [Fact]
    public void SwapOperationMove_Should_Swap_Two_Operations()
    {
        /*
         * =========================
         * 1. 创建订单
         * =========================
         */

        var order =
            new Order
            {
                Code = "ORD001",
                Priority = 1
            };


        var ticket1 =
            new JobTicket
            {
                Code = "JT001",
                Sequence = 1,
                Length = 100
            };


        var ticket2 =
            new JobTicket
            {
                Code = "JT002",
                Sequence = 2,
                Length = 100
            };


        order.JobTickets.Add(ticket1);

        order.JobTickets.Add(ticket2);


        /*
         * =========================
         * 2. 设备
         * =========================
         */

        var machine =
            new Machine
            {
                Code = "M001",

                Capabilities =
                [
                    new MachineCapability
                    {
                        MachineCode = "M001",
                        JobTicketCode = "JT001",
                        HourlyCapacity = 100,
                        SetupMinutes = 0
                    },

                    new MachineCapability
                    {
                        MachineCode = "M001",
                        JobTicketCode = "JT002",
                        HourlyCapacity = 100,
                        SetupMinutes = 0
                    }
                ]
            };


        var machines =
            new List<Machine>
            {
                machine
            };


        /*
         * =========================
         * 3. Context
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
         * 4. Timeline
         * =========================
         */

        var timeline =
            new TimelineInitializer()
                .Initialize(
                    context);


        /*
         * =========================
         * 5. 初始方案
         *
         * JT001
         * 0-2
         *
         * JT002
         * 2-4
         *
         * =========================
         */

        var solution =
            new SchedulingSolution();


        var op1 =
            new ScheduledOperation
            {
                JobTicketCode = "JT001",

                MachineCode = "M001",

                StartSlot = 0,

                DurationSlots = 2
            };


        var op2 =
            new ScheduledOperation
            {
                JobTicketCode = "JT002",

                MachineCode = "M001",

                StartSlot = 2,

                DurationSlots = 2
            };


        solution.Operations.Add(op1);

        solution.Operations.Add(op2);


        timeline.Machines["M001"]
            .Occupy(
                0,
                2);


        timeline.Machines["M001"]
            .Occupy(
                2,
                2);


        /*
         * =========================
         * 6. Index
         * =========================
         */

        var resourceIndex =
            new SchedulingResourceIndex();


        resourceIndex.Build(
            machines);


        var ticketIndex =
            new JobTicketIndex();


        ticketIndex.Build(
            context.Orders);


        /*
         * =========================
         * 7. MoveContext
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
                    resourceIndex,

                JobTicketIndex =
                    ticketIndex,

                CurrentOperation =
                    op1
            };


        /*
         * =========================
         * 8. 执行Swap
         * =========================
         */

        var move =
            new SwapOperationMove();


        var result =
            move.Apply(
                moveContext);


        /*
         * =========================
         * 9. 验证
         * =========================
         */

        Assert.True(
            result);


        var jt001 =
            solution.Operations[0];


        var jt002 =
            solution.Operations[1];


        Assert.Equal(
            "JT001",
            jt001.JobTicketCode);


        Assert.Equal(
            "JT002",
            jt002.JobTicketCode);


/*
 * JT001和JT002交换时间
 */

        Assert.Equal(
            2,
            jt001.StartSlot);


        Assert.Equal(
            0,
            jt002.StartSlot);


/*
 * Duration不变
 */

        Assert.Equal(
            2,
            jt001.DurationSlots);


        Assert.Equal(
            2,
            jt002.DurationSlots);


/*
 * Timeline保持占用
 */

        Assert.False(
            timeline.Machines["M001"]
                .IsFree(0));


        Assert.False(
            timeline.Machines["M001"]
                .IsFree(2));


        /*
         * Timeline仍然占用
         */

        Assert.False(
            timeline.Machines["M001"]
                .IsFree(0));


        Assert.False(
            timeline.Machines["M001"]
                .IsFree(2));
    }


    [Fact]
    public void SwapOperationMove_Should_Undo()
    {
        /*
         * Undo测试后续补
         *
         * 和ChangeMachineMove保持一致
         */
    }
}