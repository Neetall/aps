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
            Success =
                result.Success,


            RequestId =
                requestId,


            Message =
                result.Message,


            IsFeasible =
                result.IsFeasible,


            Warnings =
                result.Warnings
                    .ToList(),


            Evaluation =
                result.Evaluation == null
                    ? null
                    : new EvaluationDto
                    {
                        Score =
                            result.Evaluation.Score,

                        EndTime =
                            result.Evaluation.EndTime,

                        MakespanSlots =
                            result.Evaluation.MakespanSlots,

                        ProductionHours =
                            result.Evaluation.ProductionHours,

                        MachineUtilization =
                            result.Evaluation.MachineUtilization,

                        DelayCount =
                            result.Evaluation.DelayCount,

                        DelayPenalty =
                            result.Evaluation.DelayPenalty,

                        DelayMessages =
                            result.Evaluation.DelayMessages
                    },


            Operations =
                result.Items
                    .Select(x =>
                        new ScheduledOperationDto
                        {
                            OrderCode =
                                x.OrderCode,


                            JobTicketCode =
                                x.JobTicketCode,


                            MachineCode =
                                x.MachineCode,


                            ShiftPeriods =
                                x.ShiftPeriods
                                    .Select(p =>
                                        new ShiftPeriodDto
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