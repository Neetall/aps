using ProductionScheduling.Domain.Resources;

namespace ProductionScheduling.Services;

public class MachineCapabilityService
{
    public MachineCapability GetCapability(
        Machine machine,
        string jobTicketCode)
    {
        var capability =
            machine.Capabilities
                .FirstOrDefault(x =>
                    x.JobTicketCode
                    == jobTicketCode);


        if (capability == null)
            throw new Exception(
                $"设备{machine.Code}不支持派工单{jobTicketCode}");


        return capability;
    }


    public bool CanProcess(
        Machine machine,
        string jobTicketCode)
    {
        return machine.Capabilities
            .Any(x =>
                x.JobTicketCode
                == jobTicketCode);
    }
}