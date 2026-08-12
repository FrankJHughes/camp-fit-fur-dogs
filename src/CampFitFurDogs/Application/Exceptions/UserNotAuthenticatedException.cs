namespace CampFitFurDogs.Application.Exceptions;

/// <summary>
/// Represents an error condition where an operation requiring authentication
/// is invoked without an authenticated user context.
/// <para>
/// This exception is thrown when the application attempts to access the current
/// user's identity—typically through <c>ICurrentUser</c> or an authentication
/// principal—but no authenticated user is present.
/// </para>
/// <para>
/// The exception indicates an authentication failure rather than a domain or
/// validation error. It commonly occurs when authentication middleware is
/// misconfigured, missing, or bypassed, or when a caller attempts to perform an
/// action without being signed in.
/// </para>
/// </summary>
public sealed class UserNotAuthenticatedException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotAuthenticatedException"/>
    /// class with a standard error message indicating that no authenticated user
    /// was found in the current execution context.
    /// </summary>
    public UserNotAuthenticatedException()
        : base("User not authenticated.") { }
}
