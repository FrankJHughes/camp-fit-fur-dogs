namespace Frank.Core.Domain;

/// <summary>
/// Represents the base class for all domain entities.
///
/// Entities are domain objects that have a unique identity and lifecycle.
/// Unlike value objects, entities are compared by identity rather than
/// structural equality.
///
/// This class provides:
/// - Strongly typed identity via <typeparamref name="TId"/>
/// - Identity-based equality semantics
/// - A protected setter for <see cref="Id"/> to support ORM materialization
///
/// Entities should enforce domain invariants and should not expose setters
/// for mutable state except where explicitly required.
/// </summary>
/// <typeparam name="TId">
/// The strongly typed identifier for the entity, constrained to
/// <see cref="ValueObject"/>.
/// </typeparam>
public abstract class Entity<TId>
    where TId : ValueObject
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    ///
    /// The setter is protected to allow ORMs (such as EF Core) to materialize
    /// entities while preventing external mutation of the identity.
    /// </summary>
    public virtual TId Id { get; protected set; } = default!;

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    ///
    /// Entity equality is based on identity:
    /// - If the references are equal, the entities are equal.
    /// - If the identifiers are equal, the entities are equal.
    /// - Otherwise, they are not equal.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>true if the entities are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Returns a hash code for the entity based on its identity.
    /// </summary>
    /// <returns>A hash code representing the entity.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
