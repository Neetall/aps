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
        TimelineContext timeline)
    {
        var result =
            new SchedulingResult
            {
                Success =
                    solution.IsFeasible
            };


        foreach(var operation in solution.Operations)
        {
            var period =
                converter.ToPeriod(
                    timeline.TimeModel,
                    operation.StartSlot,
                    operation.DurationSlots);



            result.Items.Add(
                new ScheduledJobTicket
                {
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