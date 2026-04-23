using SharedKernel.Domain;
using SharedKernel.Events;

namespace SharedKernel.Tests.Fakes;

public sealed class FakeAggregateWithEvents : AggregateRoot<FakeAggregateId>
{
    private FakeAggregateWithEvents() { }

    private FakeAggregateWithEvents(FakeAggregateId id)
        : base()
    {
    }

    public static FakeAggregateWithEvents Create()
        => new(FakeAggregateId.New());

    public void RaiseEvent(string message)
        => RaiseDomainEvent(new FakeDomainEvent(message));

    public IReadOnlyCollection<IDomainEvent> DequeueEvents()
    {
        var events = DomainEvents.ToList();
        ClearDomainEvents();
        return events;
    }
}
