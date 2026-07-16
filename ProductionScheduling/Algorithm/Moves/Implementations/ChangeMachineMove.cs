using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Moves.Core;

namespace ProductionScheduling.Algorithm.Moves.Implementations;

public class ChangeMachineMove : IMove
{
    private readonly ScheduleDurationCalculator durationCalculator;


    public ChangeMachineMove(
        ScheduleDurationCalculator durationCalculator)
    {
        this.durationCalculator =
            durationCalculator;
    }


    public string Name =>
        "ChangeMachine";


    public bool Apply(
        MoveContext context)
    {
        var operation =
            context.CurrentOperation;


        if (operation == null)
            return false;


        var oldMachine =
            operation.MachineCode;


        var oldStart =
            operation.StartSlot;


        var oldDuration =
            operation.DurationSlots;


        var ticket =
            context.JobTicketIndex.Get(
                operation.JobTicketCode);


        if (ticket == null)
            return false;


        var capabilities =
            context.ResourceIndex.GetCapabilities(
                operation.JobTicketCode);


        foreach (var capability in capabilities)
        {
            if (capability.MachineCode ==
                oldMachine)
                continue;


            if (!context.Timeline.Machines
                    .TryGetValue(
                        capability.MachineCode,
                        out var newTimeline))
                continue;


            var duration =
                durationCalculator.Calculate(
                    ticket,
                    capability);


            var start =
                newTimeline.FindEarliest(
                    duration);


            if (start < 0)
                continue;


            var oldTimeline =
                context.Timeline.Machines
                    [oldMachine];


            oldTimeline.Release(
                oldStart,
                oldDuration);


            newTimeline.Occupy(
                start,
                duration);


            operation.MachineCode =
                capability.MachineCode;


            operation.StartSlot =
                start;


            operation.DurationSlots =
                duration;


            context.ExecutionRecord =
                new MoveExecutionRecord
                {
                    MoveName = Name,

                    Success = true,

                    JobTicketCode =
                        operation.JobTicketCode,

                    OldMachineCode =
                        oldMachine,

                    NewMachineCode =
                        capability.MachineCode,

                    OldStartSlot =
                        oldStart,

                    NewStartSlot =
                        start,

                    OldDurationSlots =
                        oldDuration,

                    NewDurationSlots =
                        duration
                };


            return true;
        }


        context.ExecutionRecord =
            new MoveExecutionRecord
            {
                MoveName = Name,
                Success = false
            };


        return false;
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


        var newTimeline =
            context.Timeline.Machines
                [record.NewMachineCode!];


        newTimeline.Release(
            record.NewStartSlot,
            record.NewDurationSlots);


        var oldTimeline =
            context.Timeline.Machines
                [record.OldMachineCode!];


        oldTimeline.Occupy(
            record.OldStartSlot,
            record.OldDurationSlots);


        operation.MachineCode =
            record.OldMachineCode!;


        operation.StartSlot =
            record.OldStartSlot;


        operation.DurationSlots =
            record.OldDurationSlots;


        context.ExecutionRecord = null;
    }
}