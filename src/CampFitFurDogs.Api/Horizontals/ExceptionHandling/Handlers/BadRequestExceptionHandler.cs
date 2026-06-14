using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.ExceptionHandling;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling.Handlers;

[ExceptionHandler(600)]
public sealed class BadRequestExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is BadRequestException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.BadRequest;

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new ProblemDetails
        {
            Title = "Bad Request",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
}
