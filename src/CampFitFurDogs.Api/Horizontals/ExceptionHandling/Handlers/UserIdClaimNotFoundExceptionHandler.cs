using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Application.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.ExceptionHandling;

[ExceptionHandler(1001)] // Runs before UnexpectedExceptionHandler
public sealed class UserIdClaimNotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is UserIdClaimNotFoundException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.InvalidUserIdentity; // Add this to your error codes

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new ProblemDetails
        {
            Title = "Invalid user identity",
            Detail = ex.Message,
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://httpstatuses.com/401"
        };
}
