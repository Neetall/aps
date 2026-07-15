namespace ProductionScheduling.Domain.Orders;

public class JobTicket
{
    /// <summary>
    ///     派工单号
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     工单号
    /// </summary>
    public string OrderCode { get; set; }

    /// <summary>
    ///     工序顺序
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    ///     总生产长度
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    ///     工厂编码
    /// </summary>
    public string FactoryCode { get; set; }
}