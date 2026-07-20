using Microsoft.AspNetCore.Mvc;
using ProductionScheduling.Api.Domain.Response;
using ProductionScheduling.Api.Services;

namespace ProductionScheduling.Api.Controllers;

[ApiController]
[Route("api/scheduling")]
public class SchedulingController : ControllerBase
{
    private readonly ISchedulingService schedulingService;


    public SchedulingController(
        ISchedulingService schedulingService)
    {
        this.schedulingService = schedulingService;
    }


    /// <summary>
    /// 执行一次排产
    /// </summary>
    [HttpPost("execute")]
    public async Task<ActionResult<SchedulingResponse>> Execute(
        [FromBody] SchedulingRequest request)
    {
        var result =
            await schedulingService.ExecuteAsync(request);

        return Ok(result);
    }
}