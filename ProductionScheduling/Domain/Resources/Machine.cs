namespace ProductionScheduling.Domain.Resources;

public class Machine
{
    /// <summary>
    ///     设备编码
    /// </summary>
    public string Code { get; set; } = string.Empty;


    /// <summary>
    /// 工厂编码
    /// </summary>
    public string FactoryCode { get; set; }
    
    /// <summary>
    ///     设备加工能力
    /// </summary>
    public List<MachineCapability> Capabilities { get; set; } = [];
}