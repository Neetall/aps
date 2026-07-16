using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Scheduling;

/// <summary>
///     最早完工时间贪心排产
///     策略:
///     1. 按订单优先级排序
///     2. 按派工单Sequence顺序排产
///     3. 每个派工单选择最早完成设备
///     4. 排产后立即占用设备时间
/// </summary>
public class GreedyScheduler : IScheduler
{
    private readonly ScheduleDurationCalculator durationCalculator;

    private readonly SchedulingResourceIndex resourceIndex;


    public GreedyScheduler(
        ScheduleDurationCalculator durationCalculator,
        SchedulingResourceIndex resourceIndex)
    {
        this.durationCalculator =
            durationCalculator;

        this.resourceIndex =
            resourceIndex;
    }


    /// <summary>
    ///     执行排产
    /// </summary>
    public SchedulingSolution Schedule(
        SchedulingContext context,
        TimelineContext timeline)
    {
        var solution =
            new SchedulingSolution();


        /*
         * 按订单优先级
         */
        var orders =
            context.Orders
                .OrderBy(x => x.Priority)
                .ToList();


        foreach (var order in orders)
        {
            /*
             * 当前订单前序工序完成时间
             *
             * 后续工序必须等待前序完成
             */
            var previousEndSlot = 0;


            foreach (var ticket in order.JobTickets
                         .OrderBy(x => x.Sequence))
            {
                var candidate =
                    FindBestMachine(
                        ticket,
                        timeline,
                        previousEndSlot);


                if (candidate == null)
                {
                    solution.IsFeasible =
                        false;

                    continue;
                }


                /*
                 * 保存排产结果
                 */
                solution.Operations
                    .Add(
                        candidate.Operation);


                /*
                 * 占用设备时间
                 */
                candidate.MachineTimeline
                    .Occupy(
                        candidate.Operation.StartSlot,
                        candidate.Operation.DurationSlots);


                /*
                 * 更新工序完成时间
                 */
                previousEndSlot =
                    candidate.EndSlot;
            }
        }


        return solution;
    }


    /// <summary>
    ///     查找最优设备
    ///     当前目标:
    ///     最早完成
    /// </summary>
    private MachineScheduleCandidate? FindBestMachine(
        JobTicket ticket,
        TimelineContext timeline,
        int earliestStartSlot)
    {
        MachineScheduleCandidate? best =
            null;


        /*
         * 根据派工单直接获取可用设备
         *
         * 不再扫描全部设备
         */
        var capabilities =
            resourceIndex
                .GetCapabilities(
                    ticket.Code);


        foreach (var capability in capabilities)
        {
            var machineCode =
                capability.MachineCode;


            if (!timeline.Machines
                    .TryGetValue(
                        machineCode,
                        out var machineTimeline))
                continue;


            /*
             * 计算生产需要Slot数量
             */
            var duration =
                durationCalculator.Calculate(
                    ticket,
                    capability);


            /*
             * 查找设备最早空闲时间
             */
            var startSlot =
                machineTimeline
                    .FindEarliest(
                        duration,
                        earliestStartSlot);


            if (startSlot < 0) continue;


            var endSlot =
                startSlot + duration;


            /*
             * 选择最早完成设备
             */
            if (best == null ||
                endSlot < best.EndSlot)
                best =
                    new MachineScheduleCandidate
                    {
                        MachineTimeline =
                            machineTimeline,


                        EndSlot =
                            endSlot,


                        Operation =
                            new ScheduledOperation
                            {
                                JobTicketCode =
                                    ticket.Code,


                                MachineCode =
                                    machineCode,


                                StartSlot =
                                    startSlot,


                                DurationSlots =
                                    duration
                            }
                    };
        }


        return best;
    }
}