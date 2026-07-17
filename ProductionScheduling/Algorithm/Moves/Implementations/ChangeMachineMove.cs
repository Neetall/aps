using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Time;

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


        if(operation == null)
            return false;



        var factory =
            context.Timelines.Get(
                operation.FactoryCode);



        var oldMachine =
            operation.MachineCode;


        var oldStart =
            operation.StartSlot;


        var oldDuration =
            operation.DurationSlots;



        var ticket =
            context.JobTicketIndex.Get(
                operation.JobTicketCode);



        if(ticket == null)
            return false;



        var capabilities =
            context.ResourceIndex.GetCapabilities(
                operation.JobTicketCode);



        foreach(var capability in capabilities)
        {
            if(capability.MachineCode ==
               oldMachine)
                continue;



            /*
             * 设备必须属于当前工厂
             */
            if(!factory.Machines
                    .TryGetValue(
                        capability.MachineCode,
                        out var newTimeline))
                continue;



            var duration =
                durationCalculator.Calculate(
                    ticket,
                    capability);



            var start =
                factory.TimeModel
                    .FindEarliestAvailable(
                        newTimeline,
                        duration);



            if(start < 0)
                continue;



            if(!factory.Machines
                    .TryGetValue(
                        oldMachine,
                        out var oldTimeline))
            {
                return false;
            }



            /*
             * 释放旧设备
             */
            oldTimeline.Release(
                oldStart,
                oldDuration);



            /*
             * 占用新设备
             */
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
                    MoveName =
                        Name,


                    Success =
                        true,


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
                        duration,


                    TabuKey =
                        TabuKeyGenerator.ChangeMachine(
                            operation.JobTicketCode,
                            oldMachine,
                            capability.MachineCode)
                };


            return true;
        }



        context.ExecutionRecord =
            new MoveExecutionRecord
            {
                MoveName =
                    Name,

                Success =
                    false
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



        if(record == null ||
           !record.Success ||
           operation == null)
            return;



        var factory =
            context.Timelines.Get(
                operation.FactoryCode);



        var newTimeline =
            factory.Machines
                [record.NewMachineCode!];


        newTimeline.Release(
            record.NewStartSlot,
            record.NewDurationSlots);



        var oldTimeline =
            factory.Machines
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



        context.ExecutionRecord =
            null;
    }
}