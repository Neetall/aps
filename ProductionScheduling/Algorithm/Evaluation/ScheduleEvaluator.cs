using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Evaluation;

public class ScheduleEvaluator
{
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
        var usedSlots =
            0;


        var totalSlotsCapacity =
            0;



        foreach(var factory in timelines.Factories.Values)
        {
            foreach(var machine in factory.Machines.Values)
            {
                usedSlots +=
                    machine.UsedSlotCount;


                totalSlotsCapacity +=
                    machine.SlotCount;
            }
        }



        result.MachineUtilization =
            totalSlotsCapacity == 0
                ? 0
                : (double)usedSlots /
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
            if(order.DueDate == null)
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
            10000;



        var delayPenalty =
            result.DelayCount *
            100000;



        var unscheduledPenalty =
            result.UnscheduledCount *
            1000000;



        var utilizationPenalty =
            (1 -
             result.MachineUtilization)
            * 100;



        return
            makespanPenalty
            +
            delayPenalty
            +
            unscheduledPenalty
            +
            utilizationPenalty;
    }
}