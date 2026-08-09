namespace Frank.Core.Domain;

/// <summary>
/// Represents the base class for all aggregate roots in the domain.
///
/// An aggregate root is the entry point for modifying a consistency boundary
/// within the domain model. All invariants of the aggregate must be enforced
/// through this root.
///
/// This class provides:
/// - Strongly typed identity via <typeparamref name="TId"/>
/// - Domain event collection and management
/// - Equality and identity semantics inherited from <see cref="Entity{TId}"/>
///
/// Domain events raised by the aggregate root are collected internally and
/// dispatched by the application layer. The domain layer does not perform
/// event dispatching.
/// </summary>
/// <typeparam name="TId">
/// The strongly typed identifier for the aggregate, constrained to
/// <see cref="AggregateId"/>.
/// </typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : AggregateId
{
    /// <summary>
    /// Internal list of domain events raised by this aggregate.
    /// These events are consumed and dispatched by the application layer.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class
    /// without assigning an identifier. This is typically used by ORMs or
    /// serialization frameworks.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class
    /// with the specified aggregate identifier.
    /// </summary>
    /// <param name="id">The strongly typed identifier for the aggregate.</param>
    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the domain events that have been raised by this aggregate.
    /// The list is read-only; events may only be added via
    /// <see cref="RaiseDomainEvent(IDomainEvent)"/>.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Raises a domain event and adds it to the internal event collection.
    ///
    /// Domain events represent meaningful occurrences within the aggregate
    /// and are later dispatched by the application layer.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all domain events raised by this aggregate.
    /// This is typically called by the application layer after dispatching
    /// the events to their respective handlers.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
