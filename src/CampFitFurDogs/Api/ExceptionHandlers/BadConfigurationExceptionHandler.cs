using CampFitFurDogs.Application.Exceptions;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Domain.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

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
