namespace Frank.Core.Domain;

/// <summary>
/// Represents the strongly-typed identifier for an aggregate root.
///
/// Aggregate IDs wrap a <see cref="Guid"/> to enforce type safety and
/// prevent accidental mixing of identifiers across different aggregates.
///
/// This class inherits from <see cref="ValueObject{Guid}"/> to ensure that
/// equality is structural and based solely on the underlying <see cref="Value"/>.
///
/// Example:
/// <code>
/// public sealed class DogId : AggregateId
/// {
///     public DogId(Guid value) : base(value) { }
/// }
/// </code>
/// </summary>
public abstract class AggregateId : ValueObject<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateId"/> class.
    /// </summary>
    /// <param name="value">The GUID value representing the aggregate's identity.</param>
    protected AggregateId(Guid value)
        : base(value)
    {
    }

    /// <summary>
    /// Returns a string representation of the aggregate ID.
    /// </summary>
    public override string ToString() => Value.ToString();
}
