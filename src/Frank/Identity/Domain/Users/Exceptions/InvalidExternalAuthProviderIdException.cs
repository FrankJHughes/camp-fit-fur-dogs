using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that an external authentication
/// provider identifier is invalid.
/// <para>
/// This exception is thrown when identity resolution or user creation receives
/// an external provider ID that is empty, malformed, or violates the domain’s
/// required format rules.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// An external identity provider returns an empty or null subject identifier.
/// </description></item>
/// <item><description>
/// The provider ID does not match the expected format for the configured
/// authentication provider.
/// </description></item>
/// <item><description>
/// A user is being linked to an external provider using an invalid identifier.
/// </description></item>
/// </list>
/// </summary>
public sealed class InvalidExternalAuthProviderIdException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="InvalidExternalAuthProviderIdException"/> with a message
    /// describing the validation failure.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of why the external provider ID is invalid.
    /// </param>
    public InvalidExternalAuthProviderIdException(string message) : base(message) { }
}
