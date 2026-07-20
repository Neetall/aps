using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Validation;

public class SchedulingSolutionValidator
{
    public void Validate(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        ValidateDuplicateJobTicket(
            solution);


        ValidateJobTicketExist(
            solution,
            context);


        ValidateMachineExist(
            solution,
            timelines);


        ValidateMachineCapability(
            solution,
            context);


        ValidateOperationRange(
            solution,
            timelines);


        ValidateMachineConflict(
            solution);


        ValidateTimelineConsistency(
            solution,
            timelines);
    }



    /// <summary>
    /// 一个JobTicket只能有一个排产结果
    /// </summary>
    private void ValidateDuplicateJobTicket(
        SchedulingSolution solution)
    {
        var duplicate =
            solution.Operations
                .GroupBy(x =>
                    x.JobTicketCode)
                .FirstOrDefault(x =>
                    x.Count() > 1);



        if(duplicate != null)
        {
            throw new InvalidOperationException(
                $"JobTicket重复:{duplicate.Key}");
        }
    }



    /// <summary>
    /// JobTicket必须存在
    /// </summary>
    private void ValidateJobTicketExist(
        SchedulingSolution solution,
        SchedulingContext context)
    {
        var tickets =
            context.Orders
                .SelectMany(x =>
                    x.JobTickets)
                .Select(x =>
                    x.Code)
                .ToHashSet();



        foreach(var operation in solution.Operations)
        {
            if(!tickets.Contains(
                    operation.JobTicketCode))
            {
                throw new InvalidOperationException(
                    $"JobTicket不存在:{operation.JobTicketCode}");
            }
        }
    }



    /// <summary>
    /// 检查设备存在
    /// </summary>
    private void ValidateMachineExist(
        SchedulingSolution solution,
        TimelineContextGroup timelines)
    {
        foreach(var operation in solution.Operations)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);



            if(!factory.Machines
                    .ContainsKey(
                        operation.MachineCode))
            {
                throw new InvalidOperationException(
                    $"设备不存在:" +
                    $"{operation.FactoryCode}/" +
                    $"{operation.MachineCode}");
            }
        }
    }



    /// <summary>
    /// 检查设备加工能力
    /// </summary>
    private void ValidateMachineCapability(
        SchedulingSolution solution,
        SchedulingContext context)
    {
        foreach(var operation in solution.Operations)
        {
            var machine =
                context.Machines
                    .FirstOrDefault(x =>
                        x.Code ==
                        operation.MachineCode);



            if(machine == null)
                continue;



            var capable =
                machine.Capabilities
                    .Any(x =>
                        x.JobTicketCode ==
                        operation.JobTicketCode);



            if(!capable)
            {
                throw new InvalidOperationException(
                    $"设备无加工能力:" +
                    $"Machine={operation.MachineCode}," +
                    $"JobTicket={operation.JobTicketCode}");
            }
        }
    }



    /// <summary>
    /// Slot范围检查
    /// </summary>
    private void ValidateOperationRange(
        SchedulingSolution solution,
        TimelineContextGroup timelines)
    {
        foreach(var operation in solution.Operations)
        {
            if(operation.StartSlot < 0)
            {
                throw new InvalidOperationException(
                    $"开始Slot非法:" +
                    $"{operation.JobTicketCode}");
            }



            if(operation.DurationSlots <= 0)
            {
                throw new InvalidOperationException(
                    $"Duration非法:" +
                    $"{operation.JobTicketCode}");
            }



            var factory =
                timelines.Get(
                    operation.FactoryCode);



            if(operation.EndSlot >
               factory.TimeModel.SlotCount)
            {
                throw new InvalidOperationException(
                    $"超出时间轴范围:" +
                    $"{operation.JobTicketCode}");
            }
        }
    }



    /// <summary>
    /// 同设备不能重叠
    /// </summary>
    private void ValidateMachineConflict(
        SchedulingSolution solution)
    {
        foreach(var group in solution.Operations
                    .GroupBy(x =>
                        new
                        {
                            x.FactoryCode,
                            x.MachineCode
                        }))
        {
            var operations =
                group
                    .OrderBy(x =>
                        x.StartSlot)
                    .ToList();



            for(var i = 1;
                i < operations.Count;
                i++)
            {
                var previous =
                    operations[i - 1];


                var current =
                    operations[i];



                if(current.StartSlot <
                   previous.EndSlot)
                {
                    throw new InvalidOperationException(
                        $"设备任务时间冲突:" +
                        $"Factory={group.Key.FactoryCode}," +
                        $"Machine={group.Key.MachineCode}," +
                        $"Job1={previous.JobTicketCode}," +
                        $"Job2={current.JobTicketCode}");
                }
            }
        }
    }



    /// <summary>
    /// Solution和Timeline一致
    /// </summary>
    private void ValidateTimelineConsistency(
        SchedulingSolution solution,
        TimelineContextGroup timelines)
    {
        foreach(var operation in solution.Operations)
        {
            var factory =
                timelines.Get(
                    operation.FactoryCode);



            var machine =
                factory.Machines[
                    operation.MachineCode];



            for(var slot =
                    operation.StartSlot;
                slot <
                    operation.EndSlot;
                slot++)
            {
                if(machine.IsFree(slot))
                {
                    throw new InvalidOperationException(
                        $"Timeline状态不一致:" +
                        $"JobTicket={operation.JobTicketCode}," +
                        $"Machine={operation.MachineCode}," +
                        $"Slot={slot}");
                }
            }
        }
    }
}