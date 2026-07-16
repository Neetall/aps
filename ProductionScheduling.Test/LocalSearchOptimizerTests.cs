using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;
using Xunit;

namespace ProductionScheduling.Test;

public class LocalSearchOptimizerTests
{
    [Fact]
    public void LocalSearch_Should_Move_To_Better_Machine()
    {
        /*
         * 1. 创建订单
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
         * 2. 创建设备
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
         * 3. Context
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
                            DateTime.Today.AddHours(8),

                        EndTime =
                            DateTime.Today.AddHours(18)
                    }
                ]
            });


        /*
         * 4. Timeline
         */
        var timeline =
            new TimelineBuilder()
                .Build(context);


        /*
         * 5. 初始方案
         *
         * JT001 -> M001
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
         * 保存原始状态
         */
        var originalMachine =
            solution.Operations[0]
                .MachineCode;


        var originalDuration =
            solution.Operations[0]
                .DurationSlots;


        /*
         * 6. Index
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
         * 7. MoveSelector
         */
        var moveSelector =
            new MoveSelector(
                new Random(1));


        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator()));


        /*
         * 8. Optimizer
         */
        var optimizer =
            new LocalSearchOptimizer(
                resourceIndex,
                ticketIndex,
                new OperationSelector(
                    new Random(1)),
                moveSelector,
                new SolutionCloner(),
                10);


        var evaluator =
            new ScheduleEvaluator();


        var before =
            evaluator.Evaluate(
                solution,
                timeline,
                context);


        /*
         * 9. 执行优化
         */
        var result =
            optimizer.Optimize(
                solution,
                context,
                timeline,
                evaluator);


        var after =
            result.Evaluation!;


        /*
         * 10. 验证优化结果
         */

        Assert.Equal(
            "M002",
            result.Solution
                .Operations[0]
                .MachineCode);


        Assert.Equal(
            1,
            result.Solution
                .Operations[0]
                .DurationSlots);


        Assert.True(
            after.Score <
            before.Score);


        /*
         * 11. 验证原方案没有污染
         */
        Assert.Equal(
            originalMachine,
            solution.Operations[0]
                .MachineCode);


        Assert.Equal(
            originalDuration,
            solution.Operations[0]
                .DurationSlots);


        Assert.False(
            result.Timeline
                .Machines[result.Solution.Operations[0].MachineCode]
                .IsFree(
                    result.Solution.Operations[0].StartSlot));
    }
}