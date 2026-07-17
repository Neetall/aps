using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Application.Result;

public class SchedulingResultConverter
{
    private readonly TimelineConverter converter;


    public SchedulingResultConverter()
    {
        converter =
            new TimelineConverter();
    }



    public SchedulingResult Convert(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        SchedulingContext context)
    {
        var result =
            new SchedulingResult
            {
                Success =
                    solution.IsFeasible
            };


        foreach(var operation in solution.Operations)
        {
            var factoryTimeline =
                timelines.Get(
                    operation.FactoryCode);



            var period =
                converter.ToPeriod(
                    factoryTimeline.TimeModel,
                    operation.StartSlot,
                    operation.DurationSlots);



            var jobTicket =
                context.Orders
                    .SelectMany(x =>
                        x.JobTickets)
                    .FirstOrDefault(x =>
                        x.Code ==
                        operation.JobTicketCode);



            if(jobTicket == null)
            {
                throw new Exception(
                    $"派工单不存在:{operation.JobTicketCode}");
            }



            result.Items.Add(
                new ScheduledJobTicket
                {
                    OrderCode =
                        jobTicket.OrderCode,

                    JobTicketCode =
                        operation.JobTicketCode,

                    MachineCode =
                        operation.MachineCode,

                    ShiftPeriods =
                    [
                        period
                    ]
                });
        }


        return result;
    }
}