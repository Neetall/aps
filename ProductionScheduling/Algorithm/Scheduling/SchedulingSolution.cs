namespace ProductionScheduling.Algorithm.Scheduling;

public class SchedulingSolution
{
    /// <summary>
    ///     排产明细
    /// </summary>
    public List<ScheduledOperation> Operations { get; set; } = [];


    /// <summary>
    ///     是否存在不可排任务
    /// </summary>
    public bool IsFeasible { get; set; } = true;


    /// <summary>
    ///     总完成时间
    ///     最大结束Slot
    /// </summary>
    public int Makespan
    {
        get
        {
            if (Operations.Count == 0)
                return 0;

            return Operations.Max(x => x.EndSlot);
        }
    }


    /// <summary>
    ///     克隆方案
    ///     给GA/SA/LNS使用
    /// </summary>
    public SchedulingSolution Clone()
    {
        var operations =
            new List<ScheduledOperation>(
                Operations.Count);


        foreach(var x in Operations)
        {
            operations.Add(
                new ScheduledOperation
                {
                    FactoryCode =
                        x.FactoryCode,

                    JobTicketCode =
                        x.JobTicketCode,

                    MachineCode =
                        x.MachineCode,

                    StartSlot =
                        x.StartSlot,

                    DurationSlots =
                        x.DurationSlots
                });
        }


        return new SchedulingSolution
        {
            IsFeasible =
                IsFeasible,

            Operations =
                operations
        };
    }
}