using Frank.Core.Domain;
using Frank.Identity.Domain.Sessions.Errors;

namespace Frank.Identity.Domain.Sessions;

/// <summary>
/// Represents the strongly‑typed identifier for a <see cref="Session"/>.
/// <para>
/// A <see cref="SessionId"/> is a domain‑validated wrapper around a <see cref="Guid"/>.
/// It enforces the invariant that session identifiers must never be
/// <see cref="Guid.Empty"/> and must always be valid GUID values.
/// </para>
/// <para>
/// This type provides factory methods, parsing helpers, and safe creation
/// patterns to ensure that invalid identifiers cannot enter the domain.
/// </para>
/// </summary>
public sealed class SessionId : AggregateId
{
    /// <summary>
    /// Initializes a new <see cref="SessionId"/> using the specified GUID value.
    /// <para>
    /// Domain rule: <see cref="Guid.Empty"/> is not permitted.
    /// </para>
    /// </summary>
    /// <param name="value">The GUID value to wrap.</param>
    /// <exception cref="InvalidSessionIdException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    private SessionId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new InvalidSessionIdException("SessionId cannot be empty.");
    }

    /// <summary>
    /// Creates a new unique <see cref="SessionId"/> using <see cref="Guid.NewGuid"/>.
    /// </summary>
    /// <returns>A new <see cref="SessionId"/> instance.</returns>
    public static SessionId New()
        => new(Guid.NewGuid());

    /// <summary>
    /// Wraps an existing GUID into a <see cref="SessionId"/>, enforcing domain invariants.
    /// </summary>
    /// <param name="value">The GUID value to wrap.</param>
    /// <returns>A new <see cref="SessionId"/> instance.</returns>
    /// <exception cref="InvalidSessionIdException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    public static SessionId From(Guid value)
        => new(value);

    /// <summary>
    /// Attempts to create a <see cref="SessionId"/> from a GUID.
    /// <para>
    /// Returns <c>true</c> if the GUID is valid (non‑empty), otherwise <c>false</c>.
    /// </para>
    /// </summary>
    /// <param name="value">The GUID value to validate.</param>
    /// <param name="sessionId">
    /// The resulting <see cref="SessionId"/> if valid; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the GUID is valid; otherwise <c>false</c>.
    /// </returns>
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
    /// Attempts to parse a raw string into a <see cref="SessionId"/>.
    /// <para>
    /// Returns <c>true</c> if the string is a valid, non‑empty GUID; otherwise <c>false</c>.
    /// </para>
    /// </summary>
    /// <param name="raw">The raw string to parse.</param>
    /// <param name="sessionId">
    /// The resulting <see cref="SessionId"/> if valid; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the string is a valid GUID and not empty; otherwise <c>false</c>.
    /// </returns>
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
