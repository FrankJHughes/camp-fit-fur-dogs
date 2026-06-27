using Frank.Abstractions.Event;

namespace Frank.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : AggregateId
{
    private readonly List<IEvent> _domainEvents = [];

    protected AggregateRoot() { }

    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    public IReadOnlyList<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
