using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that a user identity is being
/// constructed or resolved from conflicting identity sources.
/// <para>
/// This exception is thrown when multiple external identity providers supply
/// incompatible or contradictory identity information for the same user.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to merge identity data from different providers when the
/// identifiers do not match.
/// </description></item>
/// <item><description>
/// Receiving inconsistent claims (e.g., different subject IDs) during
/// user resolution.
/// </description></item>
/// <item><description>
/// Violating domain rules that require a single authoritative identity source
/// for each user.
/// </description></item>
/// </list>
/// </summary>
public class ConflictingIdentitySourcesException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ConflictingIdentitySourcesException"/> with the specified
    /// error message describing the identity conflict.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the conflicting identity sources.
    /// </param>
    public ConflictingIdentitySourcesException(string message) : base(message)
    {
    }
}
