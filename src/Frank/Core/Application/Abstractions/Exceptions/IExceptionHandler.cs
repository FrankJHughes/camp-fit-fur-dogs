using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.Exceptions;

[Registration(ServiceLifetime.Singleton)]
public interface IExceptionHandler
{
    bool CanHandle(System.Exception exception);

    IErrorCode GetErrorCode(System.Exception exception);

    ProblemDetails CreateProblemDetails(System.Exception exception);
}
