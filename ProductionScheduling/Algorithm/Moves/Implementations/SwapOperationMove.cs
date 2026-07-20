using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Scheduling;

namespace ProductionScheduling.Algorithm.Moves.Implementations;

/// <summary>
/// 交换两个派工单时间顺序
///
/// 条件:
/// 1. 同工厂
/// 2. 同设备
/// 3. 整体交换
/// 4. 不拆分
/// </summary>
public class SwapOperationMove : IMove
{
    private readonly AlgorithmDebugOptions debugOptions;


    public SwapOperationMove(
        AlgorithmDebugOptions debugOptions)
    {
        this.debugOptions =
            debugOptions;
    }



    public string Name =>
        "SwapOperation";



    public bool Apply(
        MoveContext context)
    {
        var first =
            context.CurrentOperation;



        if(first == null)
        {
            Debug(
                "SwapOperation失败:没有当前Operation");

            Fail(context);

            return false;
        }



        var second =
            FindTarget(
                context);



        if(second == null)
        {
            Debug(
                $"SwapOperation失败:没有找到交换目标:{first.JobTicketCode}");

            Fail(context);

            return false;
        }



        if(first.FactoryCode !=
           second.FactoryCode)
        {
            Debug(
                "SwapOperation失败:工厂不同");

            Fail(context);

            return false;
        }



        if(first.MachineCode !=
           second.MachineCode)
        {
            Debug(
                "SwapOperation失败:设备不同");

            Fail(context);

            return false;
        }



        Debug(
            $"SwapOperation开始: " +
            $"{first.JobTicketCode} <-> {second.JobTicketCode}, " +
            $"Machine:{first.MachineCode}");



        var factory =
            context.Timelines.Get(
                first.FactoryCode);



        if(!factory.TryGetMachine(
               first.MachineCode,
               out var timeline))
        {
            Debug(
                $"SwapOperation失败:设备不存在:{first.MachineCode}");

            Fail(context);

            return false;
        }



        var firstOldStart =
            first.StartSlot;


        var secondOldStart =
            second.StartSlot;


        var firstDuration =
            first.DurationSlots;


        var secondDuration =
            second.DurationSlots;



        timeline.Release(
            firstOldStart,
            firstDuration);


        timeline.Release(
            secondOldStart,
            secondDuration);



        var firstNewStart =
            secondOldStart;


        var secondNewStart =
            firstOldStart;



        if(!timeline.CanOccupy(
                firstNewStart,
                firstDuration)
            ||
           !timeline.CanOccupy(
                secondNewStart,
                secondDuration))
        {
            timeline.Occupy(
                firstOldStart,
                firstDuration);


            timeline.Occupy(
                secondOldStart,
                secondDuration);


            Debug(
                "SwapOperation失败:交换后时间冲突");


            Fail(context);

            return false;
        }



        timeline.Occupy(
            firstNewStart,
            firstDuration);


        timeline.Occupy(
            secondNewStart,
            secondDuration);



        first.StartSlot =
            firstNewStart;


        second.StartSlot =
            secondNewStart;



        context.ExecutionRecord =
            new MoveExecutionRecord
            {
                MoveName =
                    Name,


                Success =
                    true,


                JobTicketCode =
                    first.JobTicketCode,


                SecondJobTicketCode =
                    second.JobTicketCode,


                OldMachineCode =
                    first.MachineCode,


                SecondOldMachineCode =
                    second.MachineCode,


                OldStartSlot =
                    firstOldStart,


                NewStartSlot =
                    firstNewStart,


                SecondOldStartSlot =
                    secondOldStart,


                SecondNewStartSlot =
                    secondNewStart,


                OldDurationSlots =
                    firstDuration,


                NewDurationSlots =
                    firstDuration,


                SecondDurationSlots =
                    secondDuration,


                TabuKey =
                    TabuKeyGenerator.SwapOperation(
                        first.JobTicketCode,
                        second.JobTicketCode,
                        first.MachineCode)
            };



        Debug(
            $"SwapOperation成功: " +
            $"{first.JobTicketCode}<->{second.JobTicketCode}");



        return true;
    }



    public void Undo(
        MoveContext context)
    {
        var record =
            context.ExecutionRecord;


        if(record == null ||
           !record.Success)
            return;



        var first =
            context.Solution
                .Operations
                .FirstOrDefault(x =>
                    x.JobTicketCode ==
                    record.JobTicketCode);



        var second =
            context.Solution
                .Operations
                .FirstOrDefault(x =>
                    x.JobTicketCode ==
                    record.SecondJobTicketCode);



        if(first == null ||
           second == null)
            return;



        var factory =
            context.Timelines.Get(
                first.FactoryCode);



        if(!factory.TryGetMachine(
               first.MachineCode,
               out var timeline))
            return;



        timeline.Release(
            first.StartSlot,
            first.DurationSlots);


        timeline.Release(
            second.StartSlot,
            second.DurationSlots);



        timeline.Occupy(
            record.OldStartSlot,
            record.OldDurationSlots);


        timeline.Occupy(
            record.SecondOldStartSlot,
            record.SecondDurationSlots);



        first.StartSlot =
            record.OldStartSlot;


        second.StartSlot =
            record.SecondOldStartSlot;



        context.ExecutionRecord =
            null;
    }



    private ScheduledOperation? FindTarget(
        MoveContext context)
    {
        var current =
            context.CurrentOperation;


        foreach(var operation in
                context.Solution.Operations)
        {
            if(operation ==
               current)
                continue;


            if(operation.FactoryCode ==
               current!.FactoryCode
               &&
               operation.MachineCode ==
               current.MachineCode)
            {
                return operation;
            }
        }


        return null;
    }



    private void Fail(
        MoveContext context)
    {
        context.ExecutionRecord =
            new MoveExecutionRecord
            {
                MoveName =
                    Name,

                Success =
                    false
            };
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