using ProductionScheduling.Domain.Orders;

namespace ProductionScheduling.Algorithm.Index;

/// <summary>
///     派工单索引
///     根据派工单编码快速获取JobTicket
/// </summary>
public class JobTicketIndex
{
    private readonly Dictionary<string, JobTicket> tickets = new();


    /// <summary>
    ///     创建索引
    /// </summary>
    public void Build(
        IEnumerable<Order> orders)
    {
        tickets.Clear();


        foreach (var order in orders)
        foreach (var ticket in order.JobTickets)
            tickets[ticket.Code] =
                ticket;
    }


    /// <summary>
    ///     获取派工单
    /// </summary>
    public JobTicket? Get(
        string code)
    {
        if (tickets.TryGetValue(
                code,
                out var ticket))
            return ticket;


        return null;
    }


    /// <summary>
    ///     是否存在
    /// </summary>
    public bool Exists(
        string code)
    {
        return tickets.ContainsKey(code);
    }
}