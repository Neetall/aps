namespace ProductionScheduling.Domain.Orders;

public class Order
{
    /// <summary>
    ///     工单号
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     排产优先级
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    ///     要求完成时间
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    ///     内部派工单
    /// </summary>
    public List<JobTicket> JobTickets { get; set; } = [];
}