using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that an email address failed
/// validation according to the rules of the Identity domain.
/// <para>
/// This exception is thrown when an email is syntactically invalid, empty,
/// malformed, or otherwise fails the domain’s required format checks.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to create or update a user with an improperly formatted email.
/// </description></item>
/// <item><description>
/// Receiving invalid email data from an external identity provider.
/// </description></item>
/// <item><description>
/// Violating domain rules requiring a well-formed, non-empty email address.
/// </description></item>
/// </list>
/// </summary>
public sealed class InvalidEmailException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEmailException"/>
    /// with a message describing the validation failure.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of why the email is considered invalid.
    /// </param>
    public InvalidEmailException(string message) : base(message) { }
}
