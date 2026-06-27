using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontal.Exception.Handler;

[ExceptionHandler(600)]
public sealed class BadRequestExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is BadRequestException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.BadRequest;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Bad Request",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
}
