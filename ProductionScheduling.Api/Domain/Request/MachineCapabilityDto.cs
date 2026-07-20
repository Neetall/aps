namespace ProductionScheduling.Api.Domain.Request;

public class MachineCapabilityDto
{
    public string JobTicketCode { get; set; } = string.Empty;


    /// <summary>
    /// 米/小时
    /// </summary>
    public double HourlyCapacity { get; set; }


    /// <summary>
    /// 换型分钟
    /// </summary>
    public double SetupMinutes { get; set; }
}