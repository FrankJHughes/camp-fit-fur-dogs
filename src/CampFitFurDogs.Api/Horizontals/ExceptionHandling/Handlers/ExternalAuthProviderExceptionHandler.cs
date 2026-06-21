using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.ExceptionHandling;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling.Handlers;

[ExceptionHandler(100)]
public sealed class ExternalAuthProviderExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is ExternalAuthProviderException or AuthCallbackException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.ExternalAuthProviderFailure;

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new()
        {
            Title = "External Auth Provider Failure",
            Detail = ex.Message,
            Status = StatusCodes.Status502BadGateway,
            Type = "https://httpstatuses.com/502"
        };

}
