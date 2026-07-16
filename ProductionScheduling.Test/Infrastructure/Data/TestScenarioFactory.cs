using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Test.Infrastructure;
using ProductionScheduling.Test.Infrastructure.Data;

public static class TestScenarioFactory
{
    public static SchedulingContext Create(
        TestScenario scenario)
    {
        return scenario switch
        {
            TestScenario.Simple =>
                TestSchedulingDataFactory
                    .CreateSimpleContext(),

            TestScenario.GreedyScheduler =>
                TestSchedulingDataFactory
                    .CreateGreedySchedulerContext(),

            TestScenario.MultiOrder =>
                TestSchedulingDataFactory
                    .CreateMultiOrderContext(),

            TestScenario.MachineConflict =>
                TestSchedulingDataFactory
                    .CreateMachineConflictContext(),

            _ =>
                throw new ArgumentOutOfRangeException()
        };
    }
}