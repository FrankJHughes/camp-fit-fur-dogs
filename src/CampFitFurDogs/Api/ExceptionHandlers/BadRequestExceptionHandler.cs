using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles <see cref="BadRequestException"/> instances thrown within the
/// Camp Fit Fur Dogs application.
/// <para>
/// This exception indicates that the client has provided invalid input or made
/// a request that cannot be processed as sent. The handler converts the exception
/// into a standardized <see cref="ProblemDetails"/> response suitable for API clients.
/// </para>
/// </summary>
[ExceptionHandler(600)]
public sealed class BadRequestExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is a <see cref="BadRequestException"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is BadRequestException;

    /// <summary>
    /// Returns the error code associated with a bad request failure.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.BadRequest"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.BadRequest;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the bad request
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
            Title = "Bad Request",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
}
