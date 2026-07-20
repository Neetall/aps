using ProductionScheduling.Api.Domain.Request;

public class ScheduledOperationDto
{
    public string OrderCode { get; set; } = string.Empty;


    public string JobTicketCode { get; set; } = string.Empty;


    public string MachineCode { get; set; } = string.Empty;


    public List<ShiftPeriodDto> ShiftPeriods { get; set; } = [];
}