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

                StartSlot =
                    0,

                DurationSlots =
                    1
            });


        var timeline =
            TestTimelineFactory.CreateEmpty();


        var validator =
            new SchedulingSolutionValidator();


        Assert.Throws<InvalidOperationException>(
            () =>
                validator.Validate(
                    solution,
                    timeline));
    }



    [Fact]
    public void Validate_Should_Pass_Normal_Solution()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timeline =
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

                StartSlot =
                    0,

                DurationSlots =
                    2
            });


        timeline.Machines["M001"]
            .Occupy(
                0,
                2);



        var validator =
            new SchedulingSolutionValidator();


        validator.Validate(
            solution,
            timeline);
    }
}