using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestSchedulingDataFactory
{
    /// <summary>
    /// 创建简单测试排产数据
    ///
    /// 一个订单:
    /// ORD001
    ///
    /// 一个工序:
    /// JT001
    ///
    /// 两台设备:
    /// M001 慢
    /// M002 快
    /// </summary>
    public static SchedulingContext CreateSimpleContext()
    {
        var order =
            new Order
            {
                Code = "ORD001",
                Priority = 1
            };


        order.JobTickets.Add(
            new JobTicket
            {
                Code = "JT001",
                Sequence = 1,
                Length = 100
            });



        var machines =
            new List<Machine>
            {
                new()
                {
                    Code = "M001",

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 50,
                            SetupMinutes = 0
                        }
                    ]
                },


                new()
                {
                    Code = "M002",

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M002",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 100,
                            SetupMinutes = 0
                        }
                    ]
                }
            };



        var context =
            new SchedulingContext
            {
                Orders =
                [
                    order
                ],

                Machines =
                    machines,

                Options =
                {
                    TimeGranularityMinutes = 60
                }
            };



        context.FactoryCalendars.Add(
            new FactoryCalendar
            {
                FactoryCode = "F001",

                Periods =
                [
                    new ShiftPeriod
                    {
                        StartTime =
                            DateTime.Today.AddHours(8),

                        EndTime =
                            DateTime.Today.AddHours(18)
                    }
                ]
            });



        return context;
    }
}