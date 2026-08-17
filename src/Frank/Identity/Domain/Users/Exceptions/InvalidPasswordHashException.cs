using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that a user's password hash
/// failed validation according to the rules of the Identity domain.
/// <para>
/// This exception is thrown when a password hash is empty, malformed,
/// incorrectly formatted, or otherwise violates the domain’s required
/// hashing and storage rules.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to create or update a user with an invalid password hash.
/// </description></item>
/// <item><description>
/// Receiving malformed or incompatible password-hash data from an external
/// identity provider or legacy system.
/// </description></item>
/// <item><description>
/// Violating domain rules requiring a properly formatted, non-empty,
/// cryptographically secure password hash.
/// </description></item>
/// </list>
/// </summary>
public sealed class InvalidPasswordHashException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPasswordHashException"/>
    /// with a message describing the validation failure.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of why the password hash is considered invalid.
    /// </param>
    public InvalidPasswordHashException(string message) : base(message) { }
}
