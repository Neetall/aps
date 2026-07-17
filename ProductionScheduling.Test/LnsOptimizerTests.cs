using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace ProductionScheduling.Test;

public class LnsOptimizerTests
{
    private readonly ITestOutputHelper output;


    public LnsOptimizerTests(
        ITestOutputHelper output)
    {
        this.output =
            output;
    }



    [Fact]
    public void LnsOptimizer_Should_Destroy_And_Repair_Solution()
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



        var resourceIndex =
            TestAlgorithmFactory
                .CreateResourceIndex(
                    context);



        var ticketIndex =
            TestAlgorithmFactory
                .CreateJobTicketIndex(
                    context);



        var evaluator =
            new ScheduleEvaluator();



        var destroy =
            new WorstScoreDestroyOperator(
                evaluator,
                context);



        var repair =
            new GreedyRepairOperator(
                resourceIndex,
                ticketIndex,
                new ScheduleDurationCalculator());



        var optimizer =
            new LnsOptimizer(
                new SolutionCloner(),
                destroy,
                repair,
                new LnsAcceptance(
                    0.05,
                    new Random(1)),
                new SchedulingSolutionValidator(),
                new LnsOptions
                {
                    Iterations = 20,
                    DestroyRate = 0.5
                });



        var before =
            evaluator.Evaluate(
                solution,
                timeline,
                context);



        output.WriteLine(
            $"Before Score:{before.Score}");



        var result =
            optimizer.Optimize(
                solution,
                context,
                timeline,
                evaluator);



        TestSchedulePrinter.Print(
            output,
            "LNS Result",
            result);



        TestScheduleSolutionPrinter.Print(
            output,
            result.Solution);



        Assert.NotNull(
            result.Solution);


        Assert.NotNull(
            result.Timeline);


        Assert.NotNull(
            result.Evaluation);



        Assert.NotEmpty(
            result.Solution
                .Operations);



        output.WriteLine(
            $"After Score:{result.Evaluation!.Score}");



        Assert.Equal(
            "M001",
            solution.Operations[0]
                .MachineCode);


        Assert.Equal(
            2,
            solution.Operations[0]
                .DurationSlots);
    }
}