using CampFitFurDogs.Application.Exceptions;
using FluentValidation;
using Frank.Core.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Handles <see cref="ValidationException"/> instances thrown when incoming
/// request data fails FluentValidation rules.
/// <para>
/// This handler aggregates validation failures by property name and returns a
/// structured <see cref="ProblemDetails"/> response containing all validation
/// messages, making it easy for clients to display field‑level errors.
/// </para>
/// </summary>
[ExceptionHandler(400)]
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the given exception.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the exception is a <see cref="ValidationException"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanHandle(System.Exception ex) =>
        ex is ValidationException;

    /// <summary>
    /// Returns the error code associated with validation failures.
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// The <see cref="ErrorCode.ValidationFailed"/> error code.
    /// </returns>
    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.ValidationFailed;

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the validation
    /// failure in a structured, client‑friendly format.
    /// <para>
    /// Validation errors are grouped by property name and returned as a dictionary
    /// where each key represents a field and each value contains the associated
    /// validation messages.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception being handled.</param>
    /// <returns>
    /// A populated <see cref="ProblemDetails"/> object containing the error title,
    /// message, status code, documentation link, and detailed validation errors.
    /// </returns>
    public ProblemDetails CreateProblemDetails(System.Exception ex)
    {
        var vex = (ValidationException)ex;

        var errors = vex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return new ProblemDetails
        {
            Title = "Validation Error",
            Detail = "A validation error occurred. Please check the fields and try again.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400",
            Errors = errors
        };
    }
}
