using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles domain‑level exceptions thrown within the Camp Fit Fur Dogs application.
/// <para>
/// This includes validation failures, invariant violations, and identity‑related
/// domain errors. The handler converts these exceptions into a standardized
/// <see cref="ProblemDetails"/> response suitable for API clients.
/// </para>
/// </summary>
[ExceptionHandler(500)]
public sealed class DomainExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is one of the known domain exceptions or a
    /// general <see cref="DomainException"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is InvalidFirstNameException or
            InvalidLastNameException or
            InvalidEmailException or
            InvalidPhoneNumberException or
            InvalidExternalAuthProviderIdException or
            InvalidPasswordHashException or
            ConflictingIdentitySourcesException or
            MissingIdentitySourceException or
            DomainException;

    /// <summary>
    /// Returns the error code associated with domain‑level failures.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.DomainError"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.DomainError;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the domain error
    /// in a client‑friendly format.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Domain Error",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
}
