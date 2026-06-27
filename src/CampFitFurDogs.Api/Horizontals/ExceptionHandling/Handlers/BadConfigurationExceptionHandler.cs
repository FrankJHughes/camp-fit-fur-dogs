using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Problem;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling.Handlers;

[ExceptionHandler(200)]
public sealed class BadConfigurationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is BadConfigurationException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.BadConfiguration;

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new()
        {
            Title = "Bad Configuration",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
}
