using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Application.Options;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;
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
         * =========================
         * 1. 创建订单
         * =========================
         */

        var order =
            new Order
            {
                Code = "ORD001",

                Priority = 1,

                DueDate =
                    DateTime.Today.AddDays(1)
            };


        order.JobTickets.AddRange(
        [
            new JobTicket
            {
                Code = "JT001",

                Sequence = 1,

                Length = 100
            },

            new JobTicket
            {
                Code = "JT002",

                Sequence = 2,

                Length = 200
            }
        ]);



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
                        },

                        new MachineCapability
                        {
                            MachineCode = "M001",

                            JobTicketCode = "JT002",

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
         * 3. 创建排产上下文
         * =========================
         */

        var context =
            new SchedulingContext();


        context.Orders.Add(
            order);


        context.Machines.AddRange(
            machines);



        context.Options =
            new SchedulingOptions
            {
                TimeGranularityMinutes = 60
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
         * 4. 初始化时间轴
         * =========================
         */

        var builder =
            new TimelineBuilder();


        var timeline =
            builder.Build(
                context);



        /*
         * =========================
         * 5. 创建资源索引
         * =========================
         */

        var resourceIndex =
            new SchedulingResourceIndex();


        resourceIndex.Build(
            machines);



        /*
         * =========================
         * 6. 创建Greedy算法
         * =========================
         */

        var calculator =
            new ScheduleDurationCalculator();



        var scheduler =
            new GreedyScheduler(
                calculator,
                resourceIndex);



        /*
         * =========================
         * 7. 执行排产
         * =========================
         */

        var solution =
            scheduler.Schedule(
                context,
                timeline);



        /*
         * =========================
         * 输出排产结果
         * =========================
         */

        output.WriteLine(
            "========== 排产结果 ==========");


        foreach(var operation in solution.Operations)
        {
            var start =
                timeline.Timeline[
                    operation.StartSlot]
                    .StartTime;


            var end =
                timeline.Timeline[
                    operation.StartSlot +
                    operation.DurationSlots -
                    1]
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
         * =========================
         * 8. 验证
         * =========================
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



        /*
         * JT001应该选择M002
         */
        Assert.Equal(
            "M002",
            first.MachineCode);



        /*
         * 第二道工序必须等待第一道完成
         */
        Assert.True(
            second.StartSlot
            >=
            first.StartSlot +
            first.DurationSlots);
    }
}