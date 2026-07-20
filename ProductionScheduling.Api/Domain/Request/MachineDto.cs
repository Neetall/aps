namespace ProductionScheduling.Api.Domain.Request;

public class MachineDto
{
    public string Code { get; set; } = string.Empty;


    public string FactoryCode { get; set; } = string.Empty;


    public List<MachineCapabilityDto> Capabilities { get; set; } = [];
}