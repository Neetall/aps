namespace ProductionScheduling.Timeline;

/// <summary>
///     算法内部排产结果
/// </summary>
public class InternalScheduleItem
{
    public string JobTicketCode { get; set; }


    public string MachineCode { get; set; }


    /// <summary>
    ///     开始Slot
    /// </summary>
    public int StartSlot { get; set; }


    /// <summary>
    ///     持续Slot数量
    /// </summary>
    public int DurationSlots { get; set; }
}