using ProductionScheduling.Domain.Calendars;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestSchedulingDataFactory
{
    private const string FactoryCode =
        "F001";


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

                FactoryCode =
                    FactoryCode,

                Sequence = 1,

                Length = 100
            });


        var machines =
            new List<Machine>
            {
                new()
                {
                    Code = "M001",

                    FactoryCode =
                        FactoryCode,

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

                    FactoryCode =
                        FactoryCode,

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
                FactoryCode =
                    FactoryCode,

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



    public static SchedulingContext CreateGreedySchedulerContext()
    {
        var order =
            new Order
            {
                Code = "ORD001",

                Priority = 1,

                DueDate =
                    DateTime.Today.AddDays(1)
            };


        order.JobTickets.AddRange(
        [
            new JobTicket
            {
                Code = "JT001",

                FactoryCode =
                    FactoryCode,

                Sequence = 1,

                Length = 100
            },


            new JobTicket
            {
                Code = "JT002",

                FactoryCode =
                    FactoryCode,

                Sequence = 2,

                Length = 200
            }
        ]);


        var machines =
            new List<Machine>
            {
                new()
                {
                    Code = "M001",

                    FactoryCode =
                        FactoryCode,

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 50,
                            SetupMinutes = 0
                        },

                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT002",
                            HourlyCapacity = 50,
                            SetupMinutes = 0
                        }
                    ]
                },


                new()
                {
                    Code = "M002",

                    FactoryCode =
                        FactoryCode,

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
                FactoryCode =
                    FactoryCode,

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
    }    public static SchedulingContext CreateMultiOrderContext()
    {
        var order1 =
            new Order
            {
                Code = "ORD001",

                Priority = 1,

                DueDate =
                    DateTime.Today.AddDays(1)
            };


        order1.JobTickets.Add(
            new JobTicket
            {
                Code = "JT001",

                FactoryCode =
                    FactoryCode,

                Sequence = 1,

                Length = 100
            });



        var order2 =
            new Order
            {
                Code = "ORD002",

                Priority = 2,

                DueDate =
                    DateTime.Today.AddDays(1)
            };


        order2.JobTickets.Add(
            new JobTicket
            {
                Code = "JT002",

                FactoryCode =
                    FactoryCode,

                Sequence = 1,

                Length = 100
            });



        var machines =
            new List<Machine>
            {
                new()
                {
                    Code = "M001",

                    FactoryCode =
                        FactoryCode,

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 100,
                            SetupMinutes = 0
                        },


                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT002",
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
                    order1,
                    order2
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
                FactoryCode =
                    FactoryCode,

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



    public static SchedulingContext CreateMachineConflictContext()
    {
        var order =
            new Order
            {
                Code = "ORD001",

                Priority = 1
            };


        order.JobTickets.AddRange(
        [
            new JobTicket
            {
                Code = "JT001",

                FactoryCode =
                    FactoryCode,

                Sequence = 1,

                Length = 200
            },


            new JobTicket
            {
                Code = "JT002",

                FactoryCode =
                    FactoryCode,

                Sequence = 2,

                Length = 200
            }
        ]);



        var machines =
            new List<Machine>
            {
                new()
                {
                    Code = "M001",

                    FactoryCode =
                        FactoryCode,

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 100,
                            SetupMinutes = 0
                        },


                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT002",
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
                FactoryCode =
                    FactoryCode,

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



    public static SchedulingContext CreateSwapContext()
    {
        var order =
            new Order
            {
                Code = "ORD001",

                Priority = 1
            };


        order.JobTickets.AddRange(
        [
            new JobTicket
            {
                Code = "JT001",

                FactoryCode =
                    FactoryCode,

                Sequence = 1,

                Length = 200
            },


            new JobTicket
            {
                Code = "JT002",

                FactoryCode =
                    FactoryCode,

                Sequence = 2,

                Length = 200
            }
        ]);



        var machines =
            new List<Machine>
            {
                new()
                {
                    Code = "M001",

                    FactoryCode =
                        FactoryCode,

                    Capabilities =
                    [
                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT001",
                            HourlyCapacity = 100,
                            SetupMinutes = 0
                        },


                        new MachineCapability
                        {
                            MachineCode = "M001",
                            JobTicketCode = "JT002",
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
                FactoryCode =
                    FactoryCode,

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