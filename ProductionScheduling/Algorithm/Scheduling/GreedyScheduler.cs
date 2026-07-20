using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Scheduling;

/// <summary>
/// 最早完工时间贪心排产
///
/// 策略:
/// 1. 按订单优先级排序
/// 2. 按派工单Sequence顺序排产
/// 3. 每个派工单选择最早完成设备
/// 4. 排产后立即占用设备时间
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



    public SchedulingSolution Schedule(
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        Console.WriteLine(
            $"订单数量:{context.Orders.Count}");


        /*
         * 初始化资源索引
         *
         * 很重要:
         * API请求进入时每次都是新的Context
         */
        resourceIndex.Build(
            context.Machines);



        var solution =
            new SchedulingSolution();



        var orders =
            context.Orders
                .OrderBy(x =>
                    x.Priority)
                .ToList();



        foreach(var order in orders)
        {
            Console.WriteLine(
                $"订单:{order.Code}, 工单数量:{order.JobTickets.Count}");


            var previousEndSlot =
                0;



            foreach(var ticket in order.JobTickets
                         .OrderBy(x =>
                             x.Sequence))
            {
                Console.WriteLine(
                    $"排产工单:{ticket.Code}");



                var candidate =
                    FindBestMachine(
                        ticket,
                        timelines,
                        previousEndSlot);



                if(candidate == null)
                {
                    Console.WriteLine(
                        $"工单无法排产:{ticket.Code}");


                    solution.IsFeasible =
                        false;


                    continue;
                }



                solution.Operations.Add(
                    candidate.Operation);



                candidate.MachineTimeline.Occupy(
                    candidate.Operation.StartSlot,
                    candidate.Operation.DurationSlots);



                previousEndSlot =
                    candidate.EndSlot;
            }
        }



        solution.IsFeasible =
            solution.Operations.Count ==
            context.Orders
                .SelectMany(x =>
                    x.JobTickets)
                .Count();



        Console.WriteLine(
            $"Greedy结果数量:{solution.Operations.Count}");



        return solution;
    }



    private MachineScheduleCandidate? FindBestMachine(
        JobTicket ticket,
        TimelineContextGroup timelines,
        int earliestStartSlot)
    {
        MachineScheduleCandidate? best =
            null;



        var factoryTimeline =
            timelines.Get(
                ticket.FactoryCode);



        var capabilities =
            resourceIndex
                .GetCapabilities(
                    ticket.Code);



        Console.WriteLine(
            $"工单:{ticket.Code}, 可用设备能力:{capabilities.Count}");



        foreach(var capability in capabilities)
        {
            var machineCode =
                capability.MachineCode;



            if(!factoryTimeline.Machines
                    .TryGetValue(
                        machineCode,
                        out var machineTimeline))
            {
                Console.WriteLine(
                    $"设备不存在Timeline:{machineCode}");

                continue;
            }



            var duration =
                durationCalculator.Calculate(
                    ticket,
                    capability);



            var startSlot =
                factoryTimeline.TimeModel
                    .FindEarliestAvailable(
                        machineTimeline,
                        duration,
                        earliestStartSlot);



            if(startSlot < 0)
            {
                Console.WriteLine(
                    $"设备无可用时间:{machineCode}");

                continue;
            }



            var endSlot =
                startSlot +
                duration;



            Console.WriteLine(
                $"候选设备:{machineCode}, Start:{startSlot}, End:{endSlot}");



            if(best == null ||
               endSlot < best.EndSlot)
            {
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
                                FactoryCode =
                                    ticket.FactoryCode,


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
        }



        return best;
    }
}