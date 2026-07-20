using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Time;

namespace ProductionScheduling.Algorithm.Moves.Implementations;

public class ChangeMachineMove : IMove
{
    private readonly ScheduleDurationCalculator durationCalculator;

    private readonly AlgorithmDebugOptions debugOptions;


    public ChangeMachineMove(
        ScheduleDurationCalculator durationCalculator,
        AlgorithmDebugOptions debugOptions)
    {
        this.durationCalculator =
            durationCalculator;

        this.debugOptions =
            debugOptions;
    }



    public string Name =>
        "ChangeMachine";



    public bool Apply(
        MoveContext context)
    {
        var operation =
            context.CurrentOperation;



        if(operation == null)
        {
            Debug(
                "ChangeMachine失败:没有当前Operation");

            return false;
        }



        var factory =
            context.Timelines.Get(
                operation.FactoryCode);



        var oldMachine =
            operation.MachineCode;


        var oldStart =
            operation.StartSlot;


        var oldDuration =
            operation.DurationSlots;



        Debug(
            $"ChangeMachine开始: " +
            $"Job:{operation.JobTicketCode}, " +
            $"OldMachine:{oldMachine}, " +
            $"OldStart:{oldStart}, " +
            $"Duration:{oldDuration}");



        var ticket =
            context.JobTicketIndex.Get(
                operation.JobTicketCode);



        if(ticket == null)
        {
            Debug(
                $"ChangeMachine失败:找不到工单:{operation.JobTicketCode}");

            return false;
        }



        var capabilities =
            context.ResourceIndex.GetCapabilities(
                operation.JobTicketCode);



        Debug(
            $"可选设备数量:{capabilities.Count}");



        foreach(var capability in capabilities)
        {
            Debug(
                $"尝试设备:{capability.MachineCode}");



            if(capability.MachineCode ==
               oldMachine)
            {
                Debug(
                    "跳过当前设备");

                continue;
            }



            if(!factory.Machines
                    .TryGetValue(
                        capability.MachineCode,
                        out var newTimeline))
            {
                Debug(
                    $"目标设备不存在Timeline:{capability.MachineCode}");

                continue;
            }



            var duration =
                durationCalculator.Calculate(
                    ticket,
                    capability);



            Debug(
                $"目标设备:{capability.MachineCode}, " +
                $"新Duration:{duration}");



            var start =
                factory.TimeModel
                    .FindEarliestAvailable(
                        newTimeline,
                        duration,
                        operation.StartSlot);



            Debug(
                $"目标设备:{capability.MachineCode}, " +
                $"Start:{start}");



            if(start < 0)
            {
                Debug(
                    $"目标设备无可用时间:{capability.MachineCode}");

                continue;
            }



            if(!factory.Machines
                    .TryGetValue(
                        oldMachine,
                        out var oldTimeline))
            {
                Debug(
                    $"原设备不存在Timeline:{oldMachine}");

                return false;
            }



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



            Debug(
                $"ChangeMachine成功: " +
                $"{oldMachine}->{capability.MachineCode}, " +
                $"Start:{start}, Duration:{duration}");



            return true;
        }



        Debug(
            "ChangeMachine失败:没有找到可用设备");



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