namespace ProductionScheduling.Domain.Resources;

public class MachineCapability
{
    /// <summary>
    /// 设备编码
    /// </summary>
    public string MachineCode { get; set; }


    /// <summary>
    /// 派工单编码
    /// </summary>
    public string JobTicketCode { get; set; }


    /// <summary>
    /// 每小时生产长度
    /// 单位：米/小时
    /// </summary>
    public double HourlyCapacity { get; set; }


    /// <summary>
    /// 换型时间
    /// 单位：分钟
    /// </summary>
    public double SetupMinutes { get; set; }
}