namespace Frank.Identity.Application.Abstractions;

/// <summary>
/// Represents an exception thrown when the OIDC authentication callback pipeline
/// encounters a known failure condition.
/// <para>
/// This exception wraps an <see cref="AuthCallbackError"/> value, providing a
/// deterministic and strongly typed way to surface callback failures to upstream
/// components such as:
/// </para>
/// <list type="bullet">
/// <item><description>OIDC callback handlers</description></item>
/// <item><description>User resolution logic (<c>IUserResolver</c>)</description></item>
/// <item><description>Session creation pipelines</description></item>
/// <item><description>Middleware responsible for authentication flow control</description></item>
/// </list>
/// <para>
/// The exception message is set to <c>error.ToString()</c> for clarity and
/// consistency, ensuring that logs and telemetry reflect the exact error type.
/// </para>
/// </summary>
public sealed class AuthCallbackException : Exception
{
    /// <summary>
    /// The specific callback error that caused this exception.
    /// </summary>
    public AuthCallbackError Error { get; }

    /// <summary>
    /// Creates a new <see cref="AuthCallbackException"/> representing a failure
    /// in the OIDC authentication callback pipeline.
    /// </summary>
    /// <param name="error">
    /// The error describing the failure condition.
    /// </param>
    public AuthCallbackException(AuthCallbackError error)
        : base(error.ToString())
    {
        Error = error;
    }
}
