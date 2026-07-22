namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 邻域移动配置
/// </summary>
public sealed class MoveOptions
{
    /// <summary>
    /// 更换设备权重
    /// </summary>
    public int ChangeMachineWeight { get; set; } = 10;


    /// <summary>
    /// 时间移动权重
    /// </summary>
    public int ShiftTimeWeight { get; set; } = 3;


    /// <summary>
    /// 交换任务权重
    /// </summary>
    public int SwapOperationWeight { get; set; } = 5;
}
