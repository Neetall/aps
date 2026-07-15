namespace ProductionScheduling.Algorithm;

public class ScheduledOperation
{
    /// <summary>
    ///     派工单编码
    /// </summary>
    public string JobTicketCode { get; set; } = string.Empty;


    /// <summary>
    ///     设备编码
    /// </summary>
    public string MachineCode { get; set; } = string.Empty;


    /// <summary>
    ///     开始Slot
    /// </summary>
    public int StartSlot { get; set; }


    /// <summary>
    ///     持续Slot数量
    /// </summary>
    public int DurationSlots { get; set; }


    /// <summary>
    ///     结束Slot
    /// </summary>
    public int EndSlot =>
        StartSlot + DurationSlots;
}