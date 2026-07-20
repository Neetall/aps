namespace ProductionScheduling.Api.Domain.Request;

public class MachineCalendarDto
{
    public string FactoryCode { get; set; } = string.Empty;


    public string MachineCode { get; set; } = string.Empty;


    public List<MachineCalendarBlockDto> Blocks { get; set; } = [];
}