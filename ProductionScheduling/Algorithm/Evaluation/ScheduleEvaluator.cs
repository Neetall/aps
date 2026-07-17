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


        /*
         * 最大完工Slot
         *
         * 算法评分使用
         */
        var endSlot =
            solution.Operations
                .Max(x =>
                    x.StartSlot +
                    x.DurationSlots);



        result.MakespanSlots =
            endSlot;



        /*
         * 转换为真实时间
         *
         * 仅用于展示
         */
        if(endSlot > 0 &&
           endSlot <= timeline.Timeline.Count)
        {
            result.EndTime =
                timeline.Timeline[
                    endSlot - 1]
                .EndTime;
        }
        else
        {
            result.Score =
                double.MaxValue;

            return result;
        }



        /*
         * 总加工时间
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



        /*
         * 综合评分
         *
         * 越小越好
         */
        result.Score =
            CalculateScore(
                result);



        return result;
    }



    private double CalculateScore(
        EvaluationResult result)
    {
        /*
         * 第一目标:
         * 最早完工
         *
         * 第二目标:
         * 提高利用率
         *
         * 第三目标:
         * 延期控制
         */


        var makespan =
            result.MakespanSlots;



        var utilizationPenalty =
            (1 -
             result.MachineUtilization)
            *
            100;



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