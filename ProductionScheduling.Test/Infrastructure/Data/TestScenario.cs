namespace ProductionScheduling.Test.Infrastructure.Data;

public enum TestScenario
{
    /// <summary>
    /// 单工单快慢设备
    /// 用于Move/LS/SA
    /// </summary>
    Simple,


    /// <summary>
    /// 多工序依赖
    /// 用于Greedy Scheduler
    /// </summary>
    GreedyScheduler,


    /// <summary>
    /// 多订单优先级
    /// </summary>
    MultiOrder,


    /// <summary>
    /// 机器资源冲突
    /// </summary>
    MachineConflict
}