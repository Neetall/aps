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
         *
         * 多工厂分别统计
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



        result.Score =
            CalculateScore(
                result);



        return result;
    }



    private double CalculateScore(
        EvaluationResult result)
    {
        var makespan =
            result.MakespanSlots;


        var utilizationPenalty =
            (1 -
             result.MachineUtilization)
            * 100;


        var delayPenalty =
            result.DelayCount *
            1000;


        return
            makespan * 10000
            +
            delayPenalty
            +
            utilizationPenalty;
    }
}