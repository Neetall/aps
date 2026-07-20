using ProductionScheduling.Api.Domain.Request;
using ProductionScheduling.Api.Domain.Response;
using ProductionScheduling.Application.Result;

public static class SchedulingResponseMapper
{
    public static SchedulingResponse FromResult(
        string requestId,
        SchedulingResult result)
    {
        return new SchedulingResponse
        {
            Success = result.Success,

            RequestId = requestId,

            Message = result.Message,

            Operations =
                result.Items
                    .Select(x => new ScheduledOperationDto
                    {
                        OrderCode =
                            x.OrderCode,

                        JobTicketCode =
                            x.JobTicketCode,

                        MachineCode =
                            x.MachineCode,

                        ShiftPeriods =
                            x.ShiftPeriods
                                .Select(p => new ShiftPeriodDto
                                {
                                    StartTime =
                                        p.StartTime,

                                    EndTime =
                                        p.EndTime
                                })
                                .ToList()
                    })
                    .ToList()
        };
    }
}