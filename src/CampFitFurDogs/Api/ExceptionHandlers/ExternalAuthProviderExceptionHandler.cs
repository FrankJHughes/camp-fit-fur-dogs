using CampFitFurDogs.Application.Exceptions;
using CampFitFurDogs.Domain.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Identity.Application.Abstractions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles failures originating from external authentication providers, such as
/// OAuth/OIDC callback errors or upstream provider malfunctions.
/// <para>
/// This includes both <see cref="ExternalAuthProviderException"/> thrown by the
/// domain layer and <see cref="AuthCallbackException"/> thrown during the
/// authentication callback pipeline.
/// The handler converts these exceptions into a standardized
/// <see cref="ProblemDetails"/> response suitable for API clients.
/// </para>
/// </summary>
[ExceptionHandler(100)]
public sealed class ExternalAuthProviderExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is an <see cref="ExternalAuthProviderException"/>
    /// or <see cref="AuthCallbackException"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is ExternalAuthProviderException or AuthCallbackException;

    /// <summary>
    /// Returns the error code associated with external authentication provider failures.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.ExternalAuthProviderFailure"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.ExternalAuthProviderFailure;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the external
    /// authentication provider failure in a client‑friendly format.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "External Auth Provider Failure",
            Detail = ex.Message,
            Status = StatusCodes.Status502BadGateway,
            Type = "https://httpstatuses.com/502"
        };
}
