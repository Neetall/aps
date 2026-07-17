using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test;

public class SchedulingEngineTests
{
    [Fact]
    public void SchedulingEngine_Should_Complete_Scheduling_Flow()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var engine =
            TestEngineFactory
                .Create(
                    context);



        var result =
            engine.Execute(
                context);



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