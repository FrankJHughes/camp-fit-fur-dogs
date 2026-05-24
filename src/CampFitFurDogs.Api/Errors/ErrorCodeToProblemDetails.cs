using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Errors;

public static class ErrorCodeToProblemDetails
{
    public static ProblemDetails Create(ErrorCode code, Exception ex)
    {
        return code switch
        {
            // ───────────────────────────────────────────────
            // 502 Bad Gateway — External Auth Provider Failure
            // ───────────────────────────────────────────────
            ErrorCode.ExternalAuthProviderFailure => new ProblemDetails
            {
                Title = "External Auth Provider Failure",
                Detail = ex.Message,
                Status = StatusCodes.Status502BadGateway,
                Type = "https://httpstatuses.com/502"
            },

            // ───────────────────────────────────────────────
            // 500 Internal Server Error — Server Misconfiguration
            // ───────────────────────────────────────────────
            ErrorCode.BadConfiguration => new ProblemDetails
            {
                Title = "Bad Configuration",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.com/500"
            },

            // ───────────────────────────────────────────────
            // 409 Conflict — Duplicate Resource
            // ───────────────────────────────────────────────
            ErrorCode.DuplicateEmail => new ProblemDetails
            {
                Title = "Duplicate Email",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict,
                Type = "https://httpstatuses.com/409"
            },

            // ───────────────────────────────────────────────
            // 400 Bad Request — Request-level validation
            // ───────────────────────────────────────────────
            ErrorCode.ValidationFailed => CreateValidationProblemDetails((ValidationException)ex),

            // ───────────────────────────────────────────────
            // 400 Bad Request — Domain invariant violations
            // ───────────────────────────────────────────────
            ErrorCode.DomainError => new ProblemDetails
            {
                Title = "Domain Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400"
            },

            // ───────────────────────────────────────────────
            // 400 Bad Request — Generic bad request
            // ───────────────────────────────────────────────
            ErrorCode.BadRequest => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400"
            },

            // ───────────────────────────────────────────────
            // 500 Internal Server Error — Fallback
            // ───────────────────────────────────────────────
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
            Title = "Validation Error",
            Detail = "A validation error occurred. Please check the fields and try again.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
    }
}
