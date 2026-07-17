using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Application;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class SchedulingEngineTests
{
    [Fact]
    public void SchedulingEngine_Should_Complete_Scheduling_Flow()
    {
        /*
         * Arrange
         */

        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();



        var options =
            new SchedulingAlgorithmOptions();



        /*
         * 开启优化流水线
         *
         * 验证:
         * Greedy
         * ->
         * LocalSearch
         * ->
         * SA
         *
         * 整体流程
         */
        options.Pipeline.Add(
            new OptimizationStepOptions
            {
                Algorithm =
                    OptimizationAlgorithmType.LocalSearch,

                Enabled =
                    true,

                Order =
                    1
            });



        options.Pipeline.Add(
            new OptimizationStepOptions
            {
                Algorithm =
                    OptimizationAlgorithmType.SimulatedAnnealing,

                Enabled =
                    true,

                Order =
                    2
            });



        var engine =
            TestEngineFactory.Create(
                context,
                options);



        /*
         * Act
         */

        var result =
            engine.Execute(
                context);



        /*
         * Assert
         */

        Assert.NotNull(
            result);


        Assert.True(
            result.Success);



        Assert.NotEmpty(
            result.Items);



        Assert.Contains(
            "排产完成",
            result.Message);
    }
}