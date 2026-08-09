using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that a user identifier failed
/// validation according to the rules of the Identity domain.
/// <para>
/// This exception is thrown when a <see cref="UserId"/> is empty, malformed,
/// incorrectly formatted, or otherwise violates the domain’s required
/// invariants for user identity.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to construct a <see cref="UserId"/> from an empty or invalid GUID.
/// </description></item>
/// <item><description>
/// Receiving malformed or incompatible user-identifier data from an external
/// identity provider.
/// </description></item>
/// <item><description>
/// Violating domain rules requiring a non-empty, well-formed user identifier.
/// </description></item>
/// </list>
/// </summary>
public sealed class InvalidUserIdException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidUserIdException"/>
    /// with a message describing the validation failure.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of why the user identifier is considered invalid.
    /// </param>
    public InvalidUserIdException(string message) : base(message) { }
}
