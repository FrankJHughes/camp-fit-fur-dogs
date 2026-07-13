using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

public sealed class UserId : AggregateId
{
    private UserId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new InvalidUserIdException("UserId cannot be empty.");
    }

    /// <summary>
    /// Creates a new unique UserId.
    /// </summary>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    /// Wraps an existing Guid into a UserId, enforcing domain invariants.
    /// </summary>
    public static UserId From(Guid value) => new(value);
}
