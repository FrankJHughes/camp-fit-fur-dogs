using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.Horizontals.Exception.Handler;

[ExceptionHandler(200)]
public sealed class BadConfigurationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is BadConfigurationException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.BadConfiguration;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Bad Configuration",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
}
