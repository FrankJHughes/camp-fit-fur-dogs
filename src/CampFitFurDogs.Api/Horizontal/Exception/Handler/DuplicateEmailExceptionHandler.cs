using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontal.Exception.Handler;

[ExceptionHandler(300)]
public sealed class DuplicateEmailExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is EmailAlreadyExistsException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.DuplicateEmail;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Duplicate Email",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict,
            Type = "https://httpstatuses.com/409"
        };
}
