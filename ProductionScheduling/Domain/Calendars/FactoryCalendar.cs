namespace ProductionScheduling.Domain.Calendars;

public class FactoryCalendar
{
    /// <summary>
    ///     工厂编码
    /// </summary>
    public string FactoryCode { get; set; }

    /// <summary>
    ///     可用时间
    /// </summary>
    public List<ShiftPeriod> Periods { get; set; } = [];
}