using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Validation;

public class SchedulingSolutionValidator
{
    public void Validate(
        SchedulingSolution solution,
        TimelineContext timeline)
    {
        ValidateDuplicateJobTicket(
            solution);


        ValidateMachineExist(
            solution,
            timeline);


        ValidateOperationRange(
            solution,
            timeline);


        ValidateMachineConflict(
            solution);
    }



    /// <summary>
    ///     一个JobTicket只能存在一个排产结果
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
    ///     检查设备是否存在
    /// </summary>
    private void ValidateMachineExist(
        SchedulingSolution solution,
        TimelineContext timeline)
    {
        foreach(var operation in solution.Operations)
        {
            if(!timeline.Machines
                .ContainsKey(
                    operation.MachineCode))
            {
                throw new InvalidOperationException(
                    $"设备不存在:{operation.MachineCode}");
            }
        }
    }



    /// <summary>
    ///     检查时间范围
    /// </summary>
    private void ValidateOperationRange(
        SchedulingSolution solution,
        TimelineContext timeline)
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



            if(operation.StartSlot +
               operation.DurationSlots >
               timeline.TimeModel.SlotCount)
            {
                throw new InvalidOperationException(
                    $"超出时间轴范围:" +
                    $"{operation.JobTicketCode}");
            }
        }
    }



    /// <summary>
    ///     检查同设备任务冲突
    ///
    ///     不检查MachineTimeline
    ///     避免维护时间影响验证
    /// </summary>
    private void ValidateMachineConflict(
        SchedulingSolution solution)
    {
        foreach(var group in solution.Operations
                    .GroupBy(x =>
                        x.MachineCode))
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



                var previousEnd =
                    previous.StartSlot +
                    previous.DurationSlots;



                if(current.StartSlot <
                   previousEnd)
                {
                    throw new InvalidOperationException(
                        $"设备任务时间冲突:" +
                        $"Machine={group.Key}," +
                        $"Job1={previous.JobTicketCode}," +
                        $"Job2={current.JobTicketCode}");
                }
            }
        }
    }
}