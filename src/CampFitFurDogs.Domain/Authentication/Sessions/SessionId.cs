using SharedKernel.Domain;
using CampFitFurDogs.Domain.Authentication.Sessions.Errors;

namespace CampFitFurDogs.Domain.Authentication.Sessions;

public sealed class SessionId : AggregateId
{
    private SessionId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new InvalidSessionIdException("SessionId cannot be empty.");
    }

    /// <summary>
    /// Creates a new unique SessionId.
    /// </summary>
    public static SessionId New()
        => new(Guid.NewGuid());

    /// <summary>
    /// Wraps an existing Guid into a SessionId, enforcing domain invariants.
    /// </summary>
    public static SessionId From(Guid value)
        => new(value);

    /// <summary>
    /// Attempts to create a SessionId from a Guid.
    /// Returns true if valid, false otherwise.
    /// </summary>
    public static bool TryFrom(Guid value, out SessionId? sessionId)
    {
        if (value == Guid.Empty)
        {
            sessionId = null;
            return false;
        }

        sessionId = new SessionId(value);
        return true;
    }

    /// <summary>
    /// Attempts to parse a string into a SessionId.
    /// Returns true if valid, false otherwise.
    /// </summary>
    public static bool TryParse(string? raw, out SessionId? sessionId)
    {
        sessionId = null;

        if (raw is null)
            return false;

        if (!Guid.TryParse(raw, out var guid))
            return false;

        if (guid == Guid.Empty)
            return false;

        sessionId = new SessionId(guid);
        return true;
    }
}
