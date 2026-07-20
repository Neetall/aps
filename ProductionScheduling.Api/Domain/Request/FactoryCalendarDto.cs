namespace ProductionScheduling.Api.Domain.Request;

public class FactoryCalendarDto
{
    public string FactoryCode { get; set; } = string.Empty;


    public List<ShiftPeriodDto> Periods { get; set; } = [];
}