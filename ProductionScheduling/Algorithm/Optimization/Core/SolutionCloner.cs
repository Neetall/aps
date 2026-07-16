using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Core;

/// <summary>
///     优化状态复制器
///     用于LocalSearch、SimulatedAnnealing、TabuSearch、LNS
/// </summary>
public class SolutionCloner
{
    /// <summary>
    ///     深复制优化状态
    /// </summary>
    public ScheduleState Clone(
        ScheduleState source)
    {
        return new ScheduleState
        {
            Solution =
                source.Solution.Clone(),

            Timeline =
                CloneTimeline(
                    source.Timeline),

            Evaluation =
                CloneEvaluation(
                    source.Evaluation),
            History =
                source.History
                    .Select(x => new MoveExecutionRecord
                    {
                        MoveName =
                            x.MoveName,

                        Success =
                            x.Success,


                        JobTicketCode =
                            x.JobTicketCode,

                        SecondJobTicketCode =
                            x.SecondJobTicketCode,


                        OldMachineCode =
                            x.OldMachineCode,

                        SecondOldMachineCode =
                            x.SecondOldMachineCode,


                        OldStartSlot =
                            x.OldStartSlot,

                        NewStartSlot =
                            x.NewStartSlot,


                        SecondOldStartSlot =
                            x.SecondOldStartSlot,

                        SecondNewStartSlot =
                            x.SecondNewStartSlot,


                        OldDurationSlots =
                            x.OldDurationSlots,

                        NewDurationSlots =
                            x.NewDurationSlots,


                        SecondDurationSlots =
                            x.SecondDurationSlots,


                        OldScore =
                            x.OldScore,

                        NewScore =
                            x.NewScore,


                        Accepted =
                            x.Accepted
                    })
                    .ToList()
        };
    }


    /// <summary>
    ///     深复制时间轴状态
    ///     SchedulingTimeline共享
    ///     MachineTimeline独立复制
    /// </summary>
    private TimelineContext CloneTimeline(
        TimelineContext source)
    {
        var context =
            new TimelineContext(
                source.Timeline);


        foreach (var machine in source.Machines)
            context.AddMachineTimeline(
                machine.Value.Clone());


        return context;
    }


    /// <summary>
    ///     复制评价结果
    /// </summary>
    private EvaluationResult? CloneEvaluation(
        EvaluationResult? source)
    {
        if (source == null)
            return null;


        return new EvaluationResult
        {
            Score =
                source.Score,

            EndTime =
                source.EndTime,

            ProductionHours =
                source.ProductionHours,

            MachineUtilization =
                source.MachineUtilization,

            DelayCount =
                source.DelayCount
        };
    }
}