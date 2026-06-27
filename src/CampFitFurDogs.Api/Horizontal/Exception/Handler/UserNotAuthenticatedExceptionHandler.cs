using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Application.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontal.Exception.Handler;

[ExceptionHandler(1000)] // Runs BEFORE UnexpectedExceptionHandler
public sealed class UserNotAuthenticatedExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex)
    {
        // Your ICurrentUser throws InvalidOperationException
        // with a specific message when user is not authenticated.
        return ex is UserNotAuthenticatedException;
    }

    public IErrorCode GetErrorCode(System.Exception ex)
        => ErrorCode.UserNotAuthenticated; // You may need to add this to your error codes

    public ProblemDetails CreateProblemDetails(System.Exception ex)
        => new()
        {
            Title = "User is not authenticated",
            Detail = "Authentication is required to access this resource.",
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://httpstatuses.com/401"
        };
}
