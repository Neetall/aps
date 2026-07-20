using ProductionScheduling.Application.Options;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;

namespace ProductionScheduling.Domain.Scheduling;

public class SchedulingContext
{
    /// <summary>
    ///     工单集合
    /// </summary>
    public List<Order> Orders { get; set; } = [];


    /// <summary>
    ///     设备集合
    /// </summary>
    public List<Machine> Machines { get; set; } = [];


    /// <summary>
    ///     工厂日历
    /// </summary>
    public List<FactoryCalendar> FactoryCalendars { get; set; } = [];


    /// <summary>
    ///     设备日历
    ///     包含维修、停机等不可用时间
    /// </summary>
    public List<MachineCalendar> MachineCalendars { get; set; } = [];


    /// <summary>
    ///     排产参数
    /// </summary>
    public SchedulingOptions Options { get; set; } = new();
    
    /// <summary>
    /// 优化参数
    /// </summary>
    public SchedulingExecutionOptions ExecutionOptions { get; set; } = new();
}