using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Scheduling;

namespace ProductionScheduling.Algorithm.Moves.Implementations;

/// <summary>
///     交换两个派工单时间顺序
///     条件:
///     1. 同设备
///     2. 整体交换
///     3. 不拆分
/// </summary>
public class SwapOperationMove : IMove
{
    public string Name =>
        "SwapOperation";


    public bool Apply(
        MoveContext context)
    {
        var first =
            context.CurrentOperation;


        if (first == null)
        {
            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };

            return false;
        }


        var second =
            FindTarget(
                context);


        if (second == null)
        {
            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };

            return false;
        }


        if (first.MachineCode !=
            second.MachineCode)
        {
            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };

            return false;
        }


        var timeline =
            context.Timeline
                .Machines[first.MachineCode];


        var firstOldStart =
            first.StartSlot;


        var secondOldStart =
            second.StartSlot;


        var firstDuration =
            first.DurationSlots;


        var secondDuration =
            second.DurationSlots;


        /*
         * 释放原位置
         */
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


        if (!timeline.CanOccupy(
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


            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };


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
                    secondDuration
            };


        return true;
    }


    public void Undo(
        MoveContext context)
    {
        var record =
            context.ExecutionRecord;


        if (record == null ||
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


        if (first == null ||
            second == null)
            return;


        var timeline =
            context.Timeline
                .Machines[
                    record.OldMachineCode!];


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
        foreach (var operation in
                 context.Solution.Operations)
        {
            if (operation ==
                context.CurrentOperation)
                continue;


            if (operation.MachineCode ==
                context.CurrentOperation!
                    .MachineCode)
                return operation;
        }


        return null;
    }
}