using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Domain.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles <see cref="BadConfigurationException"/> instances thrown within the
/// Camp Fit Fur Dogs application.
/// <para>
/// This exception indicates that the application encountered an invalid or
/// missing configuration value during startup or runtime. The handler converts
/// the exception into a standardized <see cref="ProblemDetails"/> response
/// suitable for API clients.
/// </para>
/// </summary>
[ExceptionHandler(200)]
public sealed class BadConfigurationExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is a <see cref="BadConfigurationException"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is BadConfigurationException;

    /// <summary>
    /// Returns the error code associated with a bad configuration failure.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.BadConfiguration"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.BadConfiguration;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the configuration
    /// error in a client‑friendly format.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, and documentation link.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Bad Configuration",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
}
