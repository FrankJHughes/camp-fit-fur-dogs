using CampFitFurDogs.Application.Errors;
using FluentValidation;
using Frank.Abstractions.Errors;
using Frank.Abstractions.ExceptionHandling;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling.Handlers;

[ExceptionHandler(400)]
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is ValidationException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.ValidationFailed;

    public ProblemDetails CreateProblemDetails(Exception ex)
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
