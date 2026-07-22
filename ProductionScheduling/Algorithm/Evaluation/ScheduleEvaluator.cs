using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Evaluation;

public class ScheduleEvaluator
{
    private readonly EvaluationScoreOptions scoreOptions;

    public ScheduleEvaluator()
        : this(
            new EvaluationScoreOptions())
    {
    }

    public ScheduleEvaluator(
        EvaluationScoreOptions scoreOptions)
    {
        this.scoreOptions =
            scoreOptions;
    }

    public EvaluationResult Evaluate(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        SchedulingContext context)
    {
        var result =
            new EvaluationResult();



        if(solution.Operations.Count == 0)
        {
            result.Score =
                double.MaxValue;

            result.UnscheduledCount =
                solution.UnscheduledJobTickets.Count;

            return result;
        }



        DateTime? maxEndTime =
            null;


        var maxMakespanSlot =
            0;



        foreach(var operation in solution.Operations)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);



            if(operation.EndSlot >
               maxMakespanSlot)
            {
                maxMakespanSlot =
                    operation.EndSlot;
            }



            var endTime =
                factory.TimeModel
                    .GetSlotEnd(
                        operation.EndSlot - 1);



            if(maxEndTime == null ||
               endTime > maxEndTime)
            {
                maxEndTime =
                    endTime;
            }
        }



        result.MakespanSlots =
            maxMakespanSlot;


        result.EndTime =
            maxEndTime;



        /*
         * 总生产小时
         */
        var totalSlots =
            solution.Operations
                .Sum(x =>
                    x.DurationSlots);



        result.ProductionHours =
            totalSlots *
            context.Options.TimeGranularityMinutes
            /
            60.0;



        /*
         * 设备利用率
         */
        var totalSlotsCapacity =
            0;



        foreach(var factory in timelines.Factories.Values)
        {
            foreach(var machine in factory.Machines.Values)
            {
                totalSlotsCapacity +=
                    machine.SlotCount -
                    CountUnavailableSlots(
                        factory,
                        machine.MachineCode,
                        context.MachineCalendars);
            }
        }



        result.MachineUtilization =
            totalSlotsCapacity == 0
                ? 0
                : (double)totalSlots /
                  totalSlotsCapacity;



        /*
         * 未排工单数量
         */
        result.UnscheduledCount =
            solution.UnscheduledJobTickets.Count;



        /*
         * 交期检查
         */
        CalculateDelay(
            solution,
            context,
            timelines,
            result);



        result.Score =
            CalculateScore(
                result);



        return result;
    }



    private void CalculateDelay(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        EvaluationResult result)
    {
        foreach(var order in context.Orders)
        {
            if(order.DueDate == default)
                continue;



            var operations =
                solution.Operations
                    .Where(x =>
                        x.OrderCode ==
                        order.Code)
                    .ToList();



            if(operations.Count == 0)
                continue;



            DateTime? orderEnd =
                null;



            foreach(var operation in operations)
            {
                var factory =
                    timelines.Get(
                        operation.FactoryCode);



                var endTime =
                    factory.TimeModel
                        .GetSlotEnd(
                            operation.EndSlot - 1);



                if(orderEnd == null ||
                   endTime > orderEnd)
                {
                    orderEnd =
                        endTime;
                }
            }



            if(orderEnd > order.DueDate)
            {
                result.DelayCount++;


                result.DelayMessages.Add(
                    $"订单{order.Code}超过交期，完成时间:{orderEnd:yyyy-MM-dd HH:mm}");
            }
        }
    }



    private double CalculateScore(
        EvaluationResult result)
    {
        /*
         * 权重:
         *
         * 未排工单
         *      >
         * 交期延期
         *      >
         * 完工时间
         *      >
         * 利用率
         *
         */


        var makespanPenalty =
            result.MakespanSlots *
            scoreOptions.MakespanSlotWeight;



        var delayPenalty =
            result.DelayCount *
            scoreOptions.DelayCountWeight;



        var unscheduledPenalty =
            result.UnscheduledCount *
            scoreOptions.UnscheduledWeight;



        var utilizationPenalty =
            (1 -
            result.MachineUtilization)
            * scoreOptions.UtilizationWeight;



        return
            makespanPenalty
            +
            delayPenalty
            +
            unscheduledPenalty
            +
            utilizationPenalty;
    }



    private int CountUnavailableSlots(
        FactoryTimeline factory,
        string machineCode,
        IEnumerable<MachineCalendar> machineCalendars)
    {
        var unavailableSlots =
            new HashSet<int>();


        foreach(var calendar in machineCalendars)
        {
            if(calendar.FactoryCode !=
               factory.FactoryCode ||
               calendar.MachineCode !=
               machineCode)
            {
                continue;
            }


            foreach(var block in calendar.Blocks)
            {
                var startSlot =
                    factory.TimeModel.GetSlotIndex(
                        block.StartTime);


                var endSlot =
                    factory.TimeModel.GetSlotIndex(
                        block.EndTime);


                if(startSlot < 0 &&
                   endSlot < 0)
                {
                    continue;
                }


                if(startSlot < 0)
                    startSlot = 0;


                if(endSlot < 0)
                    endSlot =
                        factory.TimeModel.SlotCount;


                for(var slot = startSlot;
                    slot < endSlot;
                    slot++)
                {
                    if(factory.TimeModel.ContainsSlot(
                           slot))
                    {
                        unavailableSlots.Add(
                            slot);
                    }
                }
            }
        }


        return unavailableSlots.Count;
    }
}
