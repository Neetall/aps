using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Application.Result;

/// <summary>
/// 排产结果
///
/// 包含:
/// 1. 排产是否成功
/// 2. 是否满足全部排产约束
/// 3. 排产后的工单安排
/// 4. 算法评价指标
/// 5. 异常和提示信息
/// </summary>
public class SchedulingResult
{
    /// <summary>
    /// 是否执行成功
    ///
    /// false:
    /// - 排产过程异常
    /// - 算法执行失败
    /// - 参数错误
    ///
    /// 注意:
    /// 排产成功但存在未排工单时，
    /// 仍可能返回false
    /// </summary>
    public bool Success { get; set; }



    /// <summary>
    /// 是否满足完整排产要求
    ///
    /// true:
    /// 所有工单均已安排
    ///
    /// false:
    /// 存在无法安排的工单
    /// </summary>
    public bool IsFeasible { get; set; }



    /// <summary>
    /// 排产后的工单明细
    ///
    /// 每个元素代表:
    /// 一个派工单在指定设备上的生产时间
    /// </summary>
    public List<ScheduledJobTicket> Items { get; set; } = [];



    /// <summary>
    /// 排产评价结果
    ///
    /// 用于:
    /// - 算法优化比较
    /// - Makespan分析
    /// - 设备利用率分析
    /// - 延期分析
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }



    /// <summary>
    /// 优化执行摘要
    /// </summary>
    public OptimizationSummary? Optimization { get; set; }



    /// <summary>
    /// 已排产工单数量
    /// </summary>
    public int Count =>
        Items.Count;



    /// <summary>
    /// 所有排产任务中的最大结束时间
    ///
    /// 即当前方案预计完工时间
    /// </summary>
    public DateTime? EndTime
    {
        get
        {
            if(Items.Count == 0)
                return null;


            return Items
                .SelectMany(x =>
                    x.ShiftPeriods)
                .Max(x =>
                    x.EndTime);
        }
    }



    /// <summary>
    /// 排产执行消息
    ///
    /// 用于返回:
    /// - 成功说明
    /// - 失败原因
    /// - 系统提示
    /// </summary>
    public string? Message { get; set; }



    /// <summary>
    /// 排产警告信息
    ///
    /// 例如:
    /// - 工单JT001无法安排
    /// - 订单超过交期
    /// - 资源不足
    ///
    /// 不一定导致接口异常，
    /// 用于告诉调用方当前方案存在的问题
    /// </summary>
    public List<string> Warnings { get; set; } = [];
}
