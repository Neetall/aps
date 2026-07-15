using ProductionScheduling.Domain.Calendars;

namespace ProductionScheduling.Domain.Scheduling;

public class ScheduledJobTicket
{
    /// <summary>
    ///     工单编码
    /// </summary>
    public string OrderCode { get; set; }

    /// <summary>
    ///     派工单编码
    /// </summary>
    public string JobTicketCode { get; set; }

    /// <summary>
    ///     设备编码
    /// </summary>
    public string MachineCode { get; set; }

    /// <summary>
    ///     占用时间
    /// </summary>
    public List<ShiftPeriod> ShiftPeriods { get; set; } = [];
}