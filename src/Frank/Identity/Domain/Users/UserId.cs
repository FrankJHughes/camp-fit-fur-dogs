using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents the unique identifier for a <see cref="User"/> aggregate.
/// <para>
/// A <see cref="UserId"/> is a strongly‑typed wrapper around a <see cref="Guid"/>,
/// ensuring that only valid, non‑empty identifiers enter the domain model.
/// </para>
/// <para>
/// Domain invariants enforced:
/// </para>
/// <list type="bullet">
/// <item><description>
/// The underlying <see cref="Guid"/> must never be <see cref="Guid.Empty"/>.
/// </description></item>
/// <item><description>
/// All new identifiers must be generated internally via <see cref="New"/>.
/// </description></item>
/// <item><description>
/// External systems may wrap existing identifiers using <see cref="From"/> but
/// must still satisfy domain rules.
/// </description></item>
/// </list>
/// </summary>
public sealed class UserId : AggregateId
{
    /// <summary>
    /// Initializes a new <see cref="UserId"/> instance, enforcing the invariant
    /// that the underlying <see cref="Guid"/> cannot be empty.
    /// </summary>
    /// <param name="value">The underlying GUID value.</param>
    /// <exception cref="InvalidUserIdException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    private UserId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new InvalidUserIdException("UserId cannot be empty.");
    }

    /// <summary>
    /// Creates a new unique <see cref="UserId"/> using <see cref="Guid.NewGuid"/>.
    /// </summary>
    /// <returns>A new, valid <see cref="UserId"/> instance.</returns>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    /// Wraps an existing <see cref="Guid"/> into a <see cref="UserId"/>,
    /// enforcing domain invariants.
    /// </summary>
    /// <param name="value">The GUID to wrap.</param>
    /// <returns>A validated <see cref="UserId"/> instance.</returns>
    /// <exception cref="InvalidUserIdException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    public static UserId From(Guid value) => new(value);
}
