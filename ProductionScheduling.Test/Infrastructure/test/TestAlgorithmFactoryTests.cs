using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test.Infrastructure;

public class TestAlgorithmFactoryTests
{
    [Fact]
    public void CreatePipelineRunner_Should_Create()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        var runner =
            TestAlgorithmFactory
                .CreatePipelineRunner(
                    context);


        Assert.NotNull(
            runner);
    }
}