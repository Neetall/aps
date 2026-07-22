using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Application.Result;

public class SchedulingResultConverter
{
    private readonly TimelineConverter converter;

    private readonly JobTicketIndex? jobTicketIndex;


    public SchedulingResultConverter()
    {
        converter =
            new TimelineConverter();
    }

    public SchedulingResultConverter(
        JobTicketIndex jobTicketIndex)
        : this()
    {
        this.jobTicketIndex =
            jobTicketIndex;
    }



    public SchedulingResult Convert(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        SchedulingContext context)
    {
        var result =
            new SchedulingResult
            {
                /*
                 * Success:
                 *
                 * 表示排产流程是否正常完成
                 *
                 * 即使资源不足，
                 * 也属于正常返回
                 */
                Success =
                    true,


                /*
                 * IsFeasible:
                 *
                 * 表示是否所有工单均满足排产要求
                 */
                IsFeasible =
                    solution.IsFeasible
            };



        /*
         * 未排产工单警告
         */
        foreach(var ticket in solution.UnscheduledJobTickets)
        {
            result.Warnings.Add(
                ticket);
        }



        /*
         * 已排产工单转换
         */
        foreach(var operation in solution.Operations)
        {
            var factoryTimeline =
                timelines.Get(
                    operation.FactoryCode);



            var periods =
                converter.ToPeriods(
                    factoryTimeline.TimeModel,
                    operation.StartSlot,
                    operation.DurationSlots);



            var jobTicket =
                jobTicketIndex?.Get(
                    operation.JobTicketCode)
                ??
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
                        periods
                });
        }



        /*
         * 如果存在未完成工单，
         * 添加总体提示
         */
        if(!solution.IsFeasible)
        {
            result.Warnings.Insert(
                0,
                "当前资源无法满足全部排产要求，已返回当前约束下最佳方案");
        }



        return result;
    }
}
