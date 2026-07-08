using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Sessions.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontals.Exception.Handler;

public sealed class NotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is SessionNotFoundException ||
        ex.GetType().Name.EndsWith("NotFoundException");

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.DomainError;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Not Found",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound,
            Type = "https://httpstatuses.com/404"
        };
}
