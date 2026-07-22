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

                        ScheduleWindowMachineUtilization =
                            result.Evaluation.ScheduleWindowMachineUtilization,

                        BottleneckMachineUtilization =
                            result.Evaluation.BottleneckMachineUtilization,

                        UsedMachineCount =
                            result.Evaluation.UsedMachineCount,

                        TotalMachineCount =
                            result.Evaluation.TotalMachineCount,

                        DelayCount =
                            result.Evaluation.DelayCount,

                        DelayPenalty =
                            result.Evaluation.DelayPenalty,

                        DelayMessages =
                            result.Evaluation.DelayMessages
                    },


            Optimization =
                result.Optimization == null
                    ? null
                    : new OptimizationSummaryDto
                    {
                        Attempted =
                            result.Optimization.Attempted,

                        Effective =
                            result.Optimization.Effective,

                        TimedOut =
                            result.Optimization.TimedOut,

                        BeforeScore =
                            result.Optimization.BeforeScore,

                        AfterScore =
                            result.Optimization.AfterScore,

                        Improvement =
                            result.Optimization.Improvement,

                        ImprovementRate =
                            result.Optimization.ImprovementRate,

                        MinimumEffectiveImprovement =
                            result.Optimization.MinimumEffectiveImprovement,

                        MinimumEffectiveImprovementRate =
                            result.Optimization.MinimumEffectiveImprovementRate,

                        StartedAt =
                            result.Optimization.StartedAt,

                        EndedAt =
                            result.Optimization.EndedAt,

                        ElapsedMilliseconds =
                            result.Optimization.ElapsedMilliseconds,

                        AlgorithmResults =
                            result.Optimization.AlgorithmResults
                                .Select(x =>
                                    new OptimizationAlgorithmResultDto
                                    {
                                        Algorithm =
                                            x.Algorithm,

                                        Success =
                                            x.Success,

                                        Accepted =
                                            x.Accepted,

                                        TimedOut =
                                            x.TimedOut,

                                        BeforeScore =
                                            x.BeforeScore,

                                        AfterScore =
                                            x.AfterScore,

                                        Improvement =
                                            x.Improvement,

                                        ImprovementRate =
                                            x.ImprovementRate,

                                        StartedAt =
                                            x.StartedAt,

                                        EndedAt =
                                            x.EndedAt,

                                        ElapsedMilliseconds =
                                            x.ElapsedMilliseconds,

                                        Message =
                                            x.Message
                                    })
                                .ToList()
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
