using ProductionScheduling.Api.Domain.Request;

namespace ProductionScheduling.Api.Domain.Response;

public class SchedulingRequest
{
    /// <summary>
    /// 排产请求编号
    /// 用于追踪一次计算
    /// </summary>
    public string RequestId { get; set; } = Guid.NewGuid().ToString();


    /// <summary>
    /// 工单订单
    /// </summary>
    public List<OrderDto> Orders { get; set; } = [];


    /// <summary>
    /// 设备资源
    /// </summary>
    public List<MachineDto> Machines { get; set; } = [];


    /// <summary>
    /// 工厂日历
    /// </summary>
    public List<FactoryCalendarDto> FactoryCalendars { get; set; } = [];


    /// <summary>
    /// 设备日历
    /// 维修、停机、已有占用
    /// </summary>
    public List<MachineCalendarDto> MachineCalendars { get; set; } = [];


    /// <summary>
    /// 排产配置
    /// </summary>
    public SchedulingOptionsDto Options { get; set; } = new();
}