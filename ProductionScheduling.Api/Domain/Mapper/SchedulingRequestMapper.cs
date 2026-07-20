using ProductionScheduling.Api.Domain.Request;
using ProductionScheduling.Api.Domain.Response;
using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Api.Services;

public static class SchedulingRequestMapper
{
    public static SchedulingContext ToContext(
        SchedulingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);


        return new SchedulingContext
        {
            Orders =
                request.Orders?
                    .Select(ToOrder)
                    .ToList()
                ?? [],


            Machines =
                request.Machines?
                    .Select(ToMachine)
                    .ToList()
                ?? [],


            FactoryCalendars =
                request.FactoryCalendars?
                    .Select(ToFactoryCalendar)
                    .ToList()
                ?? [],


            MachineCalendars =
                request.MachineCalendars?
                    .Select(ToMachineCalendar)
                    .ToList()
                ?? []
        };
    }



    private static Order ToOrder(
        OrderDto dto)
    {
        var order =
            new Order
            {
                Code =
                    dto.Code,

                Priority =
                    dto.Priority,

                DueDate =
                    dto.DueDate
            };


        foreach(var ticket in dto.JobTickets ?? [])
        {
            order.JobTickets.Add(
                new JobTicket
                {
                    /*
                     * 工单所属订单
                     * 不允许外部修改
                     */
                    OrderCode =
                        dto.Code,


                    Code =
                        ticket.Code,


                    Sequence =
                        ticket.Sequence,


                    Length =
                        ticket.Length,


                    FactoryCode =
                        ticket.FactoryCode
                });
        }


        return order;
    }



    private static Machine ToMachine(
        MachineDto dto)
    {
        var machine =
            new Machine
            {
                Code =
                    dto.Code,

                FactoryCode =
                    dto.FactoryCode
            };


        foreach(var capability in dto.Capabilities ?? [])
        {
            machine.Capabilities.Add(
                new MachineCapability
                {
                    /*
                     * 能力属于设备
                     * 不能由调用方指定其他设备
                     */
                    MachineCode =
                        dto.Code,


                    JobTicketCode =
                        capability.JobTicketCode,


                    HourlyCapacity =
                        capability.HourlyCapacity,


                    SetupMinutes =
                        capability.SetupMinutes
                });
        }


        return machine;
    }



    private static FactoryCalendar ToFactoryCalendar(
        FactoryCalendarDto dto)
    {
        return new FactoryCalendar
        {
            FactoryCode =
                dto.FactoryCode,


            Periods =
                dto.Periods?
                    .Select(x =>
                        new ShiftPeriod
                        {
                            StartTime =
                                x.StartTime,

                            EndTime =
                                x.EndTime
                        })
                    .ToList()
                ?? []
        };
    }



    private static MachineCalendar ToMachineCalendar(
        MachineCalendarDto dto)
    {
        return new MachineCalendar
        {
            FactoryCode =
                dto.FactoryCode,


            MachineCode =
                dto.MachineCode,


            Blocks =
                dto.Blocks?
                    .Select(x =>
                        new MachineCalendarBlock
                        {
                            StartTime =
                                x.StartTime,


                            EndTime =
                                x.EndTime,


                            Type =
                                (MachineBlockType)x.Type,


                            Remark =
                                x.Remark
                        })
                    .ToList()
                ?? []
        };
    }
}