using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class SchedulingSolutionValidatorTests
{
    [Fact]
    public void Validate_Should_Fail_When_JobTicket_Duplicate()
    {
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



        var timelines =
            TestTimelineFactory
                .CreateEmpty();



        var validator =
            new SchedulingSolutionValidator();



        Assert.Throws<InvalidOperationException>(
            () =>
                validator.Validate(
                    solution,
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
            timelines);
    }
}