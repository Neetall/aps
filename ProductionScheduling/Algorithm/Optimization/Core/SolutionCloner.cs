using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Core;

/// <summary>
///     优化状态复制器
///
///     用于:
///     LocalSearch
///     SimulatedAnnealing
///     TabuSearch
///     LNS
/// </summary>
public class SolutionCloner
{
    public ScheduleState Clone(
        ScheduleState source)
    {
        return new ScheduleState
        {
            Solution =
                source.Solution.Clone(),

            Timelines =
                CloneTimelines(
                    source.Timelines),

            Evaluation =
                CloneEvaluation(
                    source.Evaluation),

            History =
                source.History?
                    .Select(x =>
                        CloneRecord(x))
                    .ToList()
                ??
                []
        };
    }



    /// <summary>
    /// 复制多工厂时间轴
    /// </summary>
    public TimelineContextGroup CloneTimelines(
        TimelineContextGroup source)
    {
        var result =
            new TimelineContextGroup();



        foreach(var factory in
                source.Factories.Values)
        {
            var factoryClone =
                new FactoryTimeline(
                    factory.FactoryCode,
                    factory.TimeModel);



            foreach(var machine in
                    factory.Machines.Values)
            {
                factoryClone.AddMachine(
                    machine.Clone());
            }



            result.AddFactory(
                factoryClone);
        }



        return result;
    }



    private MoveExecutionRecord CloneRecord(
        MoveExecutionRecord x)
    {
        return new MoveExecutionRecord
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
        };
    }



    public EvaluationResult? CloneEvaluation(
        EvaluationResult? source)
    {
        if(source == null)
            return null;


        return new EvaluationResult
        {
            Score =
                source.Score,


            EndTime =
                source.EndTime,


            MakespanSlots =
                source.MakespanSlots,


            ProductionHours =
                source.ProductionHours,


            MachineUtilization =
                source.MachineUtilization,


            DelayCount =
                source.DelayCount
        };
    }



    public SchedulingSolution CloneSolution(
        SchedulingSolution solution)
    {
        var result =
            new SchedulingSolution();



        foreach(var operation in
                solution.Operations)
        {
            result.Operations.Add(
                new ScheduledOperation
                {
                    FactoryCode =
                        operation.FactoryCode,


                    JobTicketCode =
                        operation.JobTicketCode,


                    MachineCode =
                        operation.MachineCode,


                    StartSlot =
                        operation.StartSlot,


                    DurationSlots =
                        operation.DurationSlots
                });
        }



        return result;
    }



    public LnsState Clone(
        LnsState source)
    {
        return new LnsState
        {
            Solution =
                source.Solution.Clone(),


            Timelines =
                CloneTimelines(
                    source.Timelines),


            Evaluation =
                CloneEvaluation(
                    source.Evaluation)
        };
    }
}