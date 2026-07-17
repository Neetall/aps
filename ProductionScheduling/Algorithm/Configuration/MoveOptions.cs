namespace ProductionScheduling.Algorithm.Configuration;

/// <summary>
/// 邻域移动配置
/// </summary>
public sealed class MoveOptions
{
    /// <summary>
    /// 更换设备权重
    /// </summary>
    public int ChangeMachineWeight { get; init; } = 10;


    /// <summary>
    /// 时间移动权重
    /// </summary>
    public int ShiftTimeWeight { get; init; } = 3;


    /// <summary>
    /// 交换任务权重
    /// </summary>
    public int SwapOperationWeight { get; init; } = 5;
}