namespace Frank.Core.Domain;

/// <summary>
/// Base class for domain events that captures common event metadata.
///
/// This class provides an <see cref="OccurredOn"/> timestamp, which must be
/// supplied by the application layer. The domain layer does not reference
/// system clocks or application abstractions, preserving domain purity.
///
/// Example:
/// <code>
/// public sealed class UserRegistered : DomainEventBase
/// {
///     public Guid UserId { get; }
///
///     public UserRegistered(Guid userId, DateTime occurredOn)
///         : base(occurredOn)
///     {
///         UserId = userId;
///     }
/// }
/// </code>
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// The moment the event occurred, supplied by the application layer.
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventBase"/> class.
    /// </summary>
    /// <param name="occurredOn">
    /// The timestamp when the event occurred. This value must be provided by
    /// the application layer to avoid domain-layer dependencies on system time.
    /// </param>
    protected DomainEventBase(DateTime occurredOn)
    {
        OccurredOn = occurredOn;
    }
}
