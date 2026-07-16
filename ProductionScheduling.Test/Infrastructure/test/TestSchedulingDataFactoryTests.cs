using ProductionScheduling.Test.Infrastructure;
using Xunit;

namespace ProductionScheduling.Test.Infrastructure;

public class TestSchedulingDataFactoryTests
{
    [Fact]
    public void CreateSimpleContext_Should_Create_Valid_Context()
    {
        var context =
            TestSchedulingDataFactory
                .CreateSimpleContext();


        Assert.Single(
            context.Orders);


        Assert.Single(
            context.Orders[0]
                .JobTickets);


        Assert.Equal(
            2,
            context.Machines.Count);


        Assert.Single(
            context.FactoryCalendars);
    }
}