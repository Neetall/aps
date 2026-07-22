namespace ProductionScheduling.Algorithm.Configuration;

public sealed class EvaluationScoreOptions
{
    /// <summary>
    /// 未排工单权重
    /// </summary>
    public double UnscheduledWeight { get; set; }
        = 1000000;

    /// <summary>
    /// 延期订单数量权重
    /// </summary>
    public double DelayCountWeight { get; set; }
        = 100000;

    /// <summary>
    /// 最大完工Slot权重
    /// </summary>
    public double MakespanSlotWeight { get; set; }
        = 10000;

    /// <summary>
    /// 设备利用率权重
    /// </summary>
    public double UtilizationWeight { get; set; }
        = 100;
}
