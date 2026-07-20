using ProductionScheduling.Api.Domain.Response;

namespace ProductionScheduling.Api.Domain.Response;

/// <summary>
/// 排产接口响应
///
/// 用于提供给外部系统(MOM/ERP等)
/// </summary>
public class SchedulingResponse
{
    /// <summary>
    /// 请求是否执行成功
    ///
    /// false:
    /// - 参数错误
    /// - 系统异常
    /// - 排产过程异常
    /// </summary>
    public bool Success { get; set; }



    /// <summary>
    /// 请求编号
    ///
    /// 用于接口调用追踪
    /// </summary>
    public string RequestId { get; set; } = string.Empty;



    /// <summary>
    /// 执行消息
    ///
    /// 例如:
    /// 排产完成
    /// 存在未排工单
    /// </summary>
    public string? Message { get; set; }



    /// <summary>
    /// 是否满足全部排产约束
    ///
    /// true:
    /// 所有工单均已安排
    ///
    /// false:
    /// 存在无法安排工单
    /// </summary>
    public bool IsFeasible { get; set; }



    /// <summary>
    /// 排产结果明细
    ///
    /// 每个元素代表:
    /// 一个派工单的设备和生产时间
    /// </summary>
    public List<ScheduledOperationDto> Operations { get; set; } = [];



    /// <summary>
    /// 排产评价指标
    ///
    /// 用于展示:
    /// - 完工时间
    /// - 利用率
    /// - 延期情况
    /// - 综合评分
    /// </summary>
    public EvaluationDto? Evaluation { get; set; }



    /// <summary>
    /// 警告信息
    ///
    /// 例如:
    /// - JT002无法安排
    /// - ORD001超过交期
    /// </summary>
    public List<string> Warnings { get; set; } = [];
}