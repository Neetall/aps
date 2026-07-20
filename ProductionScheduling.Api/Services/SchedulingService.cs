using ProductionScheduling.Api.Domain.Response;
using ProductionScheduling.Application;

namespace ProductionScheduling.Api.Services;

public class SchedulingService : ISchedulingService
{
    private readonly SchedulingEngine engine;


    public SchedulingService(
        SchedulingEngine engine)
    {
        this.engine = engine;
    }


    public async Task<SchedulingResponse> ExecuteAsync(
        SchedulingRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var context =
                SchedulingRequestMapper.ToContext(request);


            var result =
                engine.Execute(context);


            return SchedulingResponseMapper.FromResult(
                request.RequestId,
                result);
        }
        catch(Exception ex)
        {
            return new SchedulingResponse
            {
                Success = false,

                RequestId = request.RequestId,

                Message = ex.Message
            };
        }
    }
}