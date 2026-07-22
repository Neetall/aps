using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Time;

namespace ProductionScheduling.Algorithm.Moves.Implementations;

/// <summary>
/// 同设备时间移动
///
/// 不改变设备
/// 只调整开始时间
/// </summary>
public class ShiftTimeMove : IMove
{
    private readonly AlgorithmDebugOptions debugOptions;


    public ShiftTimeMove(
        AlgorithmDebugOptions debugOptions)
    {
        this.debugOptions =
            debugOptions;
    }



    public string Name =>
        "ShiftTime";



    public bool Apply(
        MoveContext context)
    {
        var operation =
            context.CurrentOperation;


        if(operation == null)
        {
            Debug(
                "ShiftTime失败:没有当前Operation");


            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,

                    Success = false
                };


            return false;
        }



        var factory =
            context.Timelines.Get(
                operation.FactoryCode);



        if(!factory.Machines
                .TryGetValue(
                    operation.MachineCode,
                    out var timeline))
        {
            Debug(
                $"ShiftTime失败:设备不存在:{operation.MachineCode}");


            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,

                    Success = false
                };


            return false;
        }



        var oldStart =
            operation.StartSlot;


        var duration =
            operation.DurationSlots;



        Debug(
            $"ShiftTime开始: " +
            $"Job:{operation.JobTicketCode}, " +
            $"Machine:{operation.MachineCode}, " +
            $"OldStart:{oldStart}, " +
            $"Duration:{duration}");



        timeline.Release(
            oldStart,
            duration);



        var newStart =
            factory.TimeModel
                .FindEarliestAvailable(
                    timeline,
                    duration,
                    oldStart + 1);



        Debug(
            $"ShiftTime查找结果:NewStart:{newStart}");



        if(newStart < 0)
        {
            timeline.Occupy(
                oldStart,
                duration);



            Debug(
                "ShiftTime失败:没有找到新的时间位置");


            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,

                    Success = false
                };


            return false;
        }



        try
        {
            timeline.Occupy(
                newStart,
                duration);
        }
        catch(InvalidOperationException ex)
        {
            timeline.Occupy(
                oldStart,
                duration);

            Debug(
                $"ShiftTime失败:目标时间不可用:{ex.Message}");

            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,

                    Success = false
                };

            return false;
        }



        operation.StartSlot =
            newStart;



        context.ExecutionRecord =
            new MoveExecutionRecord
            {
                MoveName =
                    Name,


                Success =
                    true,


                JobTicketCode =
                    operation.JobTicketCode,


                OldMachineCode =
                    operation.MachineCode,


                NewMachineCode =
                    operation.MachineCode,


                OldStartSlot =
                    oldStart,


                NewStartSlot =
                    newStart,


                OldDurationSlots =
                    duration,


                NewDurationSlots =
                    duration,


                TabuKey =
                    TabuKeyGenerator.ShiftTime(
                        operation.JobTicketCode,
                        operation.MachineCode,
                        oldStart,
                        newStart)
            };


        Debug(
            $"ShiftTime成功: " +
            $"Job:{operation.JobTicketCode}, " +
            $"Old:{oldStart}, New:{newStart}");



        return true;
    }



    public void Undo(
        MoveContext context)
    {
        var record =
            context.ExecutionRecord;


        var operation =
            context.CurrentOperation;



        if(record == null ||
           !record.Success ||
           operation == null)
            return;



        var factory =
            context.Timelines.Get(
                operation.FactoryCode);



        if(!factory.Machines
                .TryGetValue(
                    record.OldMachineCode!,
                    out var timeline))
        {
            return;
        }



        timeline.Release(
            record.NewStartSlot,
            record.NewDurationSlots);



        timeline.Occupy(
            record.OldStartSlot,
            record.OldDurationSlots);



        operation.StartSlot =
            record.OldStartSlot;


        operation.DurationSlots =
            record.OldDurationSlots;



        context.ExecutionRecord =
            null;
    }



    private void Debug(
        string message)
    {
        if(debugOptions.EnableMoveLog)
        {
            Console.WriteLine(
                message);
        }
    }
}
