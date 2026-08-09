using Frank.Core.Domain;

namespace Frank.Identity.Domain.Sessions.Errors;

/// <summary>
/// Represents a domain-level error indicating that a requested
/// <see cref="Session"/> could not be found.
/// <para>
/// This exception is thrown when session lookup operations fail to locate
/// a session for the provided identifier or token hash. It is used by
/// query handlers, middleware, and validation components that rely on
/// session existence as part of the authentication and session-management
/// workflow.
/// </para>
/// </summary>
public sealed class SessionNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionNotFoundException"/>
    /// with a standard error message indicating that the session does not exist.
    /// </summary>
    public SessionNotFoundException()
        : base("Session was not found.")
    {
    }
}
