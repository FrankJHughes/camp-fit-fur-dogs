using SharedKernel.Domain;
using SharedKernel.Events;
using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Domain;

public sealed class AggregateRootTests
{
    private sealed class TestEvent : IDomainEvent { }

    private sealed class TestAggregate : AggregateRoot<FakeAggregateId>
    {
        public TestAggregate() : base(FakeAggregateId.New()) { }

        public void Raise(IDomainEvent evt) => RaiseDomainEvent(evt);
    }

    [Fact]
    public void DomainEvents_Starts_Empty()
    {
        var agg = new TestAggregate();

        agg.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void RaiseDomainEvent_Adds_Event()
    {
        var agg = new TestAggregate();
        var evt = new TestEvent();

        agg.Raise(evt);

        agg.DomainEvents.Should().ContainSingle()
            .Which.Should().BeSameAs(evt);
    }

    [Fact]
    public void RaiseDomainEvent_Preserves_Order()
    {
        var agg = new TestAggregate();
        var e1 = new TestEvent();
        var e2 = new TestEvent();

        agg.Raise(e1);
        agg.Raise(e2);

        agg.DomainEvents.Should().HaveCount(2);
        agg.DomainEvents[0].Should().BeSameAs(e1);
        agg.DomainEvents[1].Should().BeSameAs(e2);
    }

    [Fact]
    public void ClearDomainEvents_Removes_All_Events()
    {
        var agg = new TestAggregate();
        agg.Raise(new TestEvent());

        agg.ClearDomainEvents();

        agg.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_Is_ReadOnly_Externally()
    {
        var agg = new TestAggregate();
        var evt = new TestEvent();

        agg.Raise(evt);

        Action attempt = () =>
        {
            // IReadOnlyList prevents modification, but we assert immutability anyway
            var list = (IList<IDomainEvent>)agg.DomainEvents;
            list.Clear();
        };

        attempt.Should().Throw<NotSupportedException>();
    }
}
