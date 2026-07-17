namespace ProductionScheduling.Algorithm.Optimization.Tabu;

public static class TabuKeyGenerator
{
    /// <summary>
    /// 换设备禁忌
    /// </summary>
    public static string ChangeMachine(
        string jobTicketCode,
        string? fromMachine,
        string? toMachine)
    {
        return
            $"ChangeMachine:{jobTicketCode}:{fromMachine}->{toMachine}";
    }


    /// <summary>
    /// 时间移动禁忌
    /// </summary>
    public static string ShiftTime(
        string jobTicketCode,
        string machineCode,
        int fromSlot,
        int toSlot)
    {
        return
            $"ShiftTime:{jobTicketCode}:{machineCode}:{fromSlot}->{toSlot}";
    }


    /// <summary>
    /// 交换顺序禁忌
    /// </summary>
    public static string SwapOperation(
        string firstJobTicket,
        string secondJobTicket,
        string machineCode)
    {
        /*
         * 排序保证:
         *
         * JT001-JT002
         *
         * 和
         *
         * JT002-JT001
         *
         * 是同一个交换
         */
        var jobs =
            new[]
                {
                    firstJobTicket,
                    secondJobTicket
                }
                .OrderBy(x => x)
                .ToArray();


        return
            $"SwapOperation:{machineCode}:{jobs[0]}:{jobs[1]}";
    }
}