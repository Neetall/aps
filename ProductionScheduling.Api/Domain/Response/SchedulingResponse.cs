using ProductionScheduling.Api.Domain.Request;

namespace ProductionScheduling.Api.Domain.Response;

public class SchedulingResponse
{
    public bool Success { get; set; }


    public string RequestId { get; set; } = string.Empty;


    public string Message { get; set; } = string.Empty;


    /// <summary>
    /// 排产结果
    /// </summary>
    public List<ScheduledOperationDto> Operations { get; set; } = [];


    /// <summary>
    /// 算法评价
    /// </summary>
    public SchedulingEvaluationDto Evaluation { get; set; }
}