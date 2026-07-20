using ProductionScheduling.Api.Domain.Response;

namespace ProductionScheduling.Api.Services;

public interface ISchedulingService
{
    Task<SchedulingResponse> ExecuteAsync(
        SchedulingRequest request,
        CancellationToken cancellationToken = default);
}