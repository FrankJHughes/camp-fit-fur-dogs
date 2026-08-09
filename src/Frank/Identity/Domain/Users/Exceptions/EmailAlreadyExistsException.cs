using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that an attempt was made to
/// create or register a user with an email address that already exists in
/// the system.
/// <para>
/// This exception is thrown when enforcing the uniqueness constraint on
/// owner email addresses during account creation or identity resolution.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// A new owner attempts to register with an email already associated with an existing user.
/// </description></item>
/// <item><description>
/// An external identity provider returns an email that conflicts with an existing user record.
/// </description></item>
/// </list>
/// </summary>
public sealed class EmailAlreadyExistsException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAlreadyExistsException"/>
    /// with a message describing the conflicting email address.
    /// </summary>
    /// <param name="email">
    /// The email address that already exists in the system.
    /// </param>
    public EmailAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.")
    {
    }
}
