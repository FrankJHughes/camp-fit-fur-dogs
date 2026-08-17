using Frank.Core.Domain;

namespace CampFitFurDogs.Domain.Exceptions;

/// <summary>
/// Represents a domain‑level failure originating from an external authentication
/// provider.
/// <para>
/// This exception is thrown when an external identity or authentication system
/// (such as an OIDC provider) returns an error, fails to complete the expected
/// workflow, or provides malformed or incomplete authentication data.
/// </para>
/// <para>
/// Unlike application‑layer exceptions, this type signals that the domain cannot
/// proceed because an external dependency violated assumptions required for
/// identity‑related domain operations.
/// </para>
/// </summary>
public class ExternalAuthProviderException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalAuthProviderException"/>
    /// class with a descriptive error message.
    /// </summary>
    /// <param name="message">
    /// A human‑readable description of the authentication provider failure.
    /// </param>
    public ExternalAuthProviderException(string message)
        : base(message)
    {
    }
}
