using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that a user's last name failed
/// validation according to the rules of the Identity domain.
/// <para>
/// This exception is thrown when a last name is empty, whitespace, too short,
/// too long, or otherwise fails the domain’s required formatting or content
/// rules.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to create or update a user with an invalid last name.
/// </description></item>
/// <item><description>
/// Receiving malformed or incomplete last-name data from an external
/// identity provider.
/// </description></item>
/// <item><description>
/// Violating domain rules requiring a well-formed, non-empty last name.
/// </description></item>
/// </list>
/// </summary>
public sealed class InvalidLastNameException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidLastNameException"/>
    /// with a message describing the validation failure.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of why the last name is considered invalid.
    /// </param>
    public InvalidLastNameException(string message) : base(message) { }
}
