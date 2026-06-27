using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Application.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontal.Exception.Handler;

[ExceptionHandler(1001)] // Runs before UnexpectedExceptionHandler
public sealed class UserIdClaimNotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is UserIdClaimNotFoundException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.InvalidUserIdentity; // Add this to your error codes

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Invalid user identity",
            Detail = ex.Message,
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://httpstatuses.com/401"
        };
}
