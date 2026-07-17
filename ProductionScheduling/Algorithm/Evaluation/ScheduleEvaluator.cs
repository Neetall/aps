using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Evaluation;

public class ScheduleEvaluator
{
    public EvaluationResult Evaluate(
        SchedulingSolution solution,
        TimelineContext timeline,
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



        var endSlot =
            solution.Operations
                .Max(x =>
                    x.StartSlot +
                    x.DurationSlots);



        result.MakespanSlots =
            endSlot;



        if(endSlot > 0 &&
           endSlot <= timeline.TimeModel.SlotCount)
        {
            result.EndTime =
                timeline.TimeModel.GetSlotEnd(
                    endSlot - 1);
        }
        else
        {
            result.Score =
                double.MaxValue;

            return result;
        }



        var totalSlots =
            solution.Operations
                .Sum(x =>
                    x.DurationSlots);



        result.ProductionHours =
            totalSlots *
            context.Options.TimeGranularityMinutes
            /
            60.0;



        var used =
            timeline.Machines
                .Values
                .Sum(x =>
                    x.UsedSlotCount);



        var total =
            timeline.Machines
                .Values
                .Sum(x =>
                    x.SlotCount);



        result.MachineUtilization =
            total == 0
                ? 0
                : (double)used / total;



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
            + delayPenalty
            + utilizationPenalty;
    }
}