using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Errors;

public static class ErrorCodeToProblemDetails
{
    public static ProblemDetails Create(ErrorCode code, Exception ex)
    {
        return code switch
        {
            ErrorCode.DuplicateEmail => new ProblemDetails
            {
                Title = "Duplicate Email",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict,
                Type = "https://httpstatuses.com/409"
            },

            ErrorCode.ValidationFailed => CreateValidationProblemDetails((ValidationException)ex),

            ErrorCode.DomainError => new ProblemDetails
            {
                Title = "Domain Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400"
            },

            _ => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.com/500"
            }
        };
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Title = "Validation Error", // includes "Error"
            Detail = "A validation error occurred. Please check the fields and try again.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
    }
}
