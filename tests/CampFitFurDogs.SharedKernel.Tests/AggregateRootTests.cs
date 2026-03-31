using Xunit;
using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.SharedKernel.Tests;

public class AggregateRootTests
{
    [Fact]
    public void Raising_domain_event_adds_to_collection()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething();

        Assert.Single(aggregate.DomainEvents);
        Assert.IsType<TestDomainEvent>(aggregate.DomainEvents[0]);
    }

    [Fact]
    public void Multiple_events_accumulate()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething();
        aggregate.DoSomething();
        aggregate.DoSomething();

        Assert.Equal(3, aggregate.DomainEvents.Count);
    }

    [Fact]
    public void Clear_removes_all_events()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.DoSomething();
        aggregate.DoSomething();

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void New_aggregate_has_no_events()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        Assert.Empty(aggregate.DomainEvents);
    }
}