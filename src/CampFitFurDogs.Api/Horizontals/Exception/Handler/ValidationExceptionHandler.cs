using CampFitFurDogs.Application.Errors;
using FluentValidation;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontals.Exception.Handler;

[ExceptionHandler(400)]
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is ValidationException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.ValidationFailed;

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
