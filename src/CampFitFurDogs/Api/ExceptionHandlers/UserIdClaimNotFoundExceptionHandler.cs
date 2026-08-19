using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles <see cref="UserIdClaimNotFoundException"/> instances thrown when the
/// authentication system fails to locate the required user identifier claim.
/// <para>
/// This typically indicates a malformed identity token, an improperly configured
/// external authentication provider, or an unexpected identity mapping failure.
/// The handler converts the exception into a standardized <see cref="ProblemDetails"/>
/// response suitable for API clients.
/// </para>
/// </summary>
[ExceptionHandler(1001)] // Runs before UnexpectedExceptionHandler
public sealed class UserIdClaimNotFoundExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is a <see cref="UserIdClaimNotFoundException"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is UserIdClaimNotFoundException;

    /// <summary>
    /// Returns the error code associated with missing or invalid user identity claims.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.InvalidUserIdentity"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.InvalidUserIdentity;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the identity claim
    /// failure in a client‑friendly format.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Invalid user identity",
            Detail = ex.Message,
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://httpstatuses.com/401"
        };
}
