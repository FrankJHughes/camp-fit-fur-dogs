using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles <see cref="UserNotAuthenticatedException"/> instances thrown when an
/// operation requires authentication but the current user context does not contain
/// a valid authenticated identity.
/// <para>
/// This typically occurs when <c>ICurrentUser</c> is accessed without a valid
/// authentication token, or when middleware responsible for establishing the user
/// principal has not run or has failed.
/// The handler converts the exception into a standardized <see cref="ProblemDetails"/>
/// response suitable for API clients.
/// </para>
/// </summary>
[ExceptionHandler(1000)] // Runs BEFORE UnexpectedExceptionHandler
public sealed class UserNotAuthenticatedExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is a <see cref="UserNotAuthenticatedException"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex)
    {
        // ICurrentUser throws UserNotAuthenticatedException when no authenticated user exists.
        return ex is UserNotAuthenticatedException;
    }

    /// <summary>
    /// Returns the error code associated with missing authentication.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.UserNotAuthenticated"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex)
        => ErrorCode.UserNotAuthenticated;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the authentication
    /// failure in a client‑friendly format.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex)
        => new()
        {
            Title = "User is not authenticated",
            Detail = "Authentication is required to access this resource.",
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://httpstatuses.com/401"
        };
}
