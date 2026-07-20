namespace ProductionScheduling.Api.Domain.Request;

public class OrderDto
{
    public string Code { get; set; } = string.Empty;


    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; set; }


    public DateTime DueDate { get; set; }


    public List<JobTicketDto> JobTickets { get; set; } = [];
}