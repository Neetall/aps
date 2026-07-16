using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Application.Result;

/// <summary>
///     排产结果
/// </summary>
public class SchedulingResult
{
    /// <summary>
    ///     是否成功
    /// </summary>
    public bool Success { get; set; }


    /// <summary>
    ///     排产明细
    /// </summary>
    public List<ScheduledJobTicket> Items { get; set; } = [];


    /// <summary>
    ///     评价结果
    ///     用于算法分析
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }


    /// <summary>
    ///     排产数量
    /// </summary>
    public int Count =>
        Items.Count;


    /// <summary>
    ///     计算完成时间
    /// </summary>
    public DateTime? EndTime
    {
        get
        {
            if (Items.Count == 0)
                return null;

            return Items
                .SelectMany(x => x.ShiftPeriods)
                .Max(x => x.EndTime);
        }
    }


    /// <summary>
    ///     消息
    /// </summary>
    public string? Message { get; set; }
}