using Xunit;

namespace ProductionScheduling.Test.Infrastructure.test;

public class TestTimelineFactoryTests
{
    [Fact]
    public void Create_Should_Create_Timeline()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var timeline =
            TestTimelineFactory
                .Create(context);


        Assert.NotNull(
            timeline);


        Assert.NotEmpty(
            timeline.Machines);


        Assert.True(
            timeline.Machines.ContainsKey(
                "M001"));


        Assert.True(
            timeline.Machines.ContainsKey(
                "M002"));
    }
}