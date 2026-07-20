namespace ProductionScheduling.Api.Domain.Request;

public class JobTicketDto
{
    public string Code { get; set; } = string.Empty;


    public int Sequence { get; set; }


    /// <summary>
    /// 生产长度
    /// </summary>
    public double Length { get; set; }


    public string FactoryCode { get; set; } = string.Empty;
}