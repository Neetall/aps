using ProductionScheduling.Algorithm.Constraints;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;
using ProductionScheduling.Test.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ProductionScheduling.Test;

public class SchedulingSolutionValidatorTests
{
    [Fact]
    public void Validate_From_DI_Should_Fail_When_Order_Precedence_Broken()
    {
        var services =
            new ServiceCollection();


        services.AddProductionScheduling();


        using var provider =
            services.BuildServiceProvider();


        var validator =
            provider.GetRequiredService<SchedulingSolutionValidator>();


        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        context.Orders[0]
            .JobTickets
            .Add(
                new JobTicket
                {
                    Code = "JT002",
                    FactoryCode = "F001",
                    Sequence = 2,
                    Length = 100
                });


        context.Machines[0]
            .Capabilities
            .Add(
                new MachineCapability
                {
                    MachineCode = "M001",
                    JobTicketCode = "JT002",
                    HourlyCapacity = 50,
                    SetupMinutes = 0
                });


        var timelines =
            TestTimelineFactory
                .Create(
                    context);


        var solution =
            new SchedulingSolution
            {
                Operations =
                [
                    new ScheduledOperation
                    {
                        JobTicketCode = "JT001",
                        MachineCode = "M001",
                        FactoryCode = "F001",
                        StartSlot = 2,
                        DurationSlots = 1
                    },
                    new ScheduledOperation
                    {
                        JobTicketCode = "JT002",
                        MachineCode = "M001",
                        FactoryCode = "F001",
                        StartSlot = 0,
                        DurationSlots = 1
                    }
                ]
            };


        Assert.Throws<ConstraintViolationException>(
            () =>
                validator.Validate(
                    solution,
                    context,
                    timelines));
    }



    [Fact]
    public void Validate_Should_Fail_When_JobTicket_Duplicate()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                FactoryCode =
                    "F001",

                StartSlot =
                    0,

                DurationSlots =
                    2
            });


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M002",

                FactoryCode =
                    "F001",

                StartSlot =
                    0,

                DurationSlots =
                    1
            });



        var validator =
            new SchedulingSolutionValidator();



        Assert.Throws<ConstraintViolationException>(
            () =>
                validator.Validate(
                    solution,
                    context,
                    timelines));
    }



    [Fact]
    public void Validate_Should_Fail_When_Machine_Conflict()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        context.Orders[0]
            .JobTickets
            .Add(
                new JobTicket
                {
                    Code = "JT002",
                    FactoryCode = "F001",
                    Sequence = 2,
                    Length = 100
                });


        context.Machines[0]
            .Capabilities
            .Add(
                new MachineCapability
                {
                    MachineCode = "M001",
                    JobTicketCode = "JT002",
                    HourlyCapacity = 50,
                    SetupMinutes = 0
                });



        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                FactoryCode =
                    "F001",

                StartSlot =
                    0,

                DurationSlots =
                    3
            });


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT002",

                MachineCode =
                    "M001",

                FactoryCode =
                    "F001",

                StartSlot =
                    2,

                DurationSlots =
                    2
            });



        var validator =
            new SchedulingSolutionValidator();



        Assert.Throws<ConstraintViolationException>(
            () =>
                validator.Validate(
                    solution,
                    context,
                    timelines));
    }



    [Fact]
    public void Validate_Should_Fail_When_Operation_Out_Of_Range()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();



        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var factoryTimeline =
            timelines.Factories["F001"];



        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                FactoryCode =
                    "F001",

                StartSlot =
                    factoryTimeline.TimeModel.SlotCount - 1,

                DurationSlots =
                    5
            });



        var validator =
            new SchedulingSolutionValidator();



        Assert.Throws<ConstraintViolationException>(
            () =>
                validator.Validate(
                    solution,
                    context,
                    timelines));
    }



    [Fact]
    public void Validate_Should_Pass_Normal_Solution()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timelines =
            TestTimelineFactory
                .Create(
                    context);



        var solution =
            new SchedulingSolution();


        solution.Operations.Add(
            new ScheduledOperation
            {
                JobTicketCode =
                    "JT001",

                MachineCode =
                    "M001",

                FactoryCode =
                    "F001",

                StartSlot =
                    0,

                DurationSlots =
                    2
            });



        timelines.Factories["F001"]
            .Machines["M001"]
            .Occupy(
                0,
                2);



        var validator =
            new SchedulingSolutionValidator();



        validator.Validate(
            solution,
            context,
            timelines);
    }
}
