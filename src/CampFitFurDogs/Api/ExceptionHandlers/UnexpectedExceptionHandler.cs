using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Acts as the final catch‑all exception handler for any unhandled or unexpected
/// exceptions thrown within the Camp Fit Fur Dogs application.
/// <para>
/// This handler ensures that all exceptions—regardless of type—are converted into
/// a standardized <see cref="ProblemDetails"/> response, preventing raw stack traces
/// or framework‑level errors from leaking to API clients.
/// </para>
/// </summary>
[ExceptionHandler(int.MaxValue)]
public sealed class UnexpectedExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// <para>
    /// Because this is the fallback handler, it always returns <c>true</c>.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>Always <c>true</c>.</returns>
    public bool CanHandle(System.Exception ex) => true;

    /// <summary>
    /// Returns the error code associated with unexpected failures.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.Unexpected"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.Unexpected;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the unexpected
    /// error in a client‑friendly format.
    /// <para>
    /// The response uses a <c>500 Internal Server Error</c> status code and includes
    /// the exception message for diagnostic purposes.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Internal Server Error",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
}
