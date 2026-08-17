using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that a required identity source
/// was missing during user resolution or construction.
/// <para>
/// This exception is thrown when the Identity domain expects an external or
/// internal identity source (e.g., OIDC provider data, local identity data,
/// or merged identity claims) but none is present.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to construct a user from external authentication data when the
/// provider did not supply the required identity fields.
/// </description></item>
/// <item><description>
/// Attempting to resolve a user identity when no authoritative identity source
/// is available.
/// </description></item>
/// <item><description>
/// Violating domain rules requiring at least one valid identity source for
/// user creation or update flows.
/// </description></item>
/// </list>
/// </summary>
public class MissingIdentitySourceException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIdentitySourceException"/>
    /// with a message describing the missing identity source.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the missing identity source condition.
    /// </param>
    public MissingIdentitySourceException(string message) : base(message)
    {
    }
}
