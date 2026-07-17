using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Tabu;

namespace ProductionScheduling.Algorithm.Moves.Implementations;

/// <summary>
///     同设备时间移动
///     不改变设备
///     只调整开始时间
/// </summary>
public class ShiftTimeMove : IMove
{
    public string Name =>
        "ShiftTime";


    public bool Apply(
        MoveContext context)
    {
        var operation =
            context.CurrentOperation;


        if (operation == null)
        {
            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };

            return false;
        }


        if (!context.Timeline.Machines
                .TryGetValue(
                    operation.MachineCode,
                    out var timeline))
        {
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


        timeline.Release(
            oldStart,
            duration);


        var newStart =
            timeline.FindEarliest(
                duration,
                oldStart + 1);


        if (newStart < 0)
        {
            timeline.Occupy(
                oldStart,
                duration);


            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,
                    Success = false
                };


            return false;
        }


        timeline.Occupy(
            newStart,
            duration);


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


        return true;
    }


    public void Undo(
        MoveContext context)
    {
        var record =
            context.ExecutionRecord;


        var operation =
            context.CurrentOperation;


        if (record == null ||
            !record.Success ||
            operation == null)
            return;


        if (!context.Timeline.Machines
                .TryGetValue(
                    record.OldMachineCode!,
                    out var timeline))
            return;


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
}