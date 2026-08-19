using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Identity.Domain.Users.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles <see cref="EmailAlreadyExistsException"/> instances thrown when a user
/// attempts to register or update an account using an email address that already
/// exists in the system.
/// <para>
/// This handler converts the exception into a standardized <see cref="ProblemDetails"/>
/// response, returning a <c>409 Conflict</c> status code to indicate that the
/// requested operation cannot be completed due to an email uniqueness violation.
/// </para>
/// </summary>
[ExceptionHandler(300)]
public sealed class DuplicateEmailExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is an <see cref="EmailAlreadyExistsException"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is EmailAlreadyExistsException;

    /// <summary>
    /// Returns the error code associated with duplicate email failures.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.DuplicateEmail"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.DuplicateEmail;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the duplicate
    /// email error in a client‑friendly format.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Duplicate Email",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict,
            Type = "https://httpstatuses.com/409"
        };
}
