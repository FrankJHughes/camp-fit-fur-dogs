namespace CampFitFurDogs.Application.Exceptions;

/// <summary>
/// Represents an error condition where the authenticated user's identity does
/// not contain the required <c>UserId</c> claim.
/// <para>
/// This exception is thrown when the application attempts to resolve the
/// current user's identifier (typically via <c>ICurrentUser</c>) but the
/// underlying authentication principal does not provide a valid user ID claim.
/// </para>
/// <para>
/// The exception indicates a structural authentication failure rather than a
/// domain or validation error. It is typically associated with misconfigured
/// identity providers, missing claims, or improperly constructed authentication
/// tokens.
/// </para>
/// </summary>
public sealed class UserIdClaimNotFoundException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdClaimNotFoundException"/>
    /// class with a standard error message indicating that the user ID claim
    /// could not be located.
    /// </summary>
    public UserIdClaimNotFoundException()
        : base("User ID claim not found.") { }
}
