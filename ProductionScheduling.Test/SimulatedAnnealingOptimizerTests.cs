using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class SimulatedAnnealingOptimizerTests
{
    [Fact]
    public void SimulatedAnnealing_Should_Return_Better_Or_Equal_Solution()
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


        var ticket =
            new JobTicket
            {
                Code = "JT001",
                Sequence = 1,
                Length = 100
            };


        order.JobTickets.Add(
            ticket);


        /*
         * =========================
         * 2. 创建设备
         *
         * M001慢
         * M002快
         *
         * =========================
         */

        var machines =
            new List<Machine>
            {
                new()
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


                new()
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
         * 3. SchedulingContext
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
         * 故意放慢设备
         *
         * JT001 -> M001
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
         * 7. 注册Move
         * =========================
         */

        var moveSelector =
            new MoveSelector(
                new Random(1));


        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()),
            10);


        moveSelector.Register(
            new ShiftTimeMove(),
            3);


        /*
         * =========================
         * 8. 创建SA
         * =========================
         */

        var optimizer =
            new SimulatedAnnealingOptimizer(
                resourceIndex,
                ticketIndex,
                new OperationSelector(new Random(1)),
                moveSelector,
                new SolutionCloner(),
                new AcceptanceCriteria(new Random(1)),
                100,
                100);


        var evaluator =
            new ScheduleEvaluator();


        var before =
            evaluator.Evaluate(
                solution,
                timeline,
                context);


        /*
         * =========================
         * 9. 执行SA
         * =========================
         */

        var result =
            optimizer.Optimize(
                solution,
                context,
                timeline,
                evaluator);


        /*
         * =========================
         * 10. 验证结果
         * =========================
         */

        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timeline);


        Assert.NotNull(
            result.Evaluation);


        Assert.True(
            result.Solution.IsFeasible);


        /*
         * SA不能比初始差
         */
        Assert.True(
            result.Evaluation.Score
            <=
            before.Score);


        /*
         * 应该迁移到快设备
         */
        var operation =
            result.Solution
                .Operations[0];


        Assert.Equal(
            "M002",
            operation.MachineCode);


        /*
         * M002:
         *
         * 100 / 100 = 1小时
         */
        Assert.Equal(
            1,
            operation.DurationSlots);


        /*
         * Timeline一致
         */
        Assert.False(
            result.Timeline
                .Machines[operation.MachineCode]
                .IsFree(
                    operation.StartSlot));


        /*
         * 原设备释放
         */
        Assert.True(
            result.Timeline
                .Machines["M001"]
                .IsFree(0));
    }
}