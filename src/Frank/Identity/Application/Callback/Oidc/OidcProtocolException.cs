namespace Frank.Identity.Application.Callback.Oidc;

/// <summary>
/// Represents an exception thrown when an OIDC protocol violation or unexpected
/// condition occurs during the authentication callback pipeline.
/// <para>
/// This exception is used to surface protocol‑level failures such as missing
/// required fields, invalid token structures, or incomplete pipeline results.
/// </para>
/// <para>
/// Unlike <see cref="AuthCallbackException"/>, which wraps structured error
/// codes, <see cref="OidcProtocolException"/> is intended for unexpected or
/// unrecoverable protocol inconsistencies that indicate the identity provider
/// or callback flow behaved outside expected norms.
/// </para>
/// </summary>
public sealed class OidcProtocolException : Exception
{
    /// <summary>
    /// Creates a new <see cref="OidcProtocolException"/> with the specified
    /// error message describing the protocol violation.
    /// </summary>
    /// <param name="message">
    /// A human‑readable description of the protocol error encountered.
    /// </param>
    public OidcProtocolException(string message) : base(message) { }
}
