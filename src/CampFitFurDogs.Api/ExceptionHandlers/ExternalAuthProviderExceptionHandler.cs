using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;
using Frank.Application.Abstractions.Identity;

namespace CampFitFurDogs.Api.ExceptionHandlers;

[ExceptionHandler(100)]
public sealed class ExternalAuthProviderExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is ExternalAuthProviderException or AuthCallbackException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.ExternalAuthProviderFailure;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "External Auth Provider Failure",
            Detail = ex.Message,
            Status = StatusCodes.Status502BadGateway,
            Type = "https://httpstatuses.com/502"
        };

}
