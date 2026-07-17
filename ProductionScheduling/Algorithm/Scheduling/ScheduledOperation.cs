public class ScheduledOperation
{
    public string FactoryCode { get; set; } = string.Empty;

    public string JobTicketCode { get; set; } = string.Empty;

    public string MachineCode { get; set; } = string.Empty;

    public int StartSlot { get; set; }

    public int DurationSlots { get; set; }


    public int EndSlot =>
        StartSlot + DurationSlots;
}