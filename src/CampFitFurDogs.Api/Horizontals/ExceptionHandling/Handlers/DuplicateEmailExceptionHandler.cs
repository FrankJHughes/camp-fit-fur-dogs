using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.ExceptionHandling;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling.Handlers;

[ExceptionHandler(300)]
public sealed class DuplicateEmailExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is EmailAlreadyExistsException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.DuplicateEmail;

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new ProblemDetails
        {
            Title = "Duplicate Email",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict,
            Type = "https://httpstatuses.com/409"
        };
}
