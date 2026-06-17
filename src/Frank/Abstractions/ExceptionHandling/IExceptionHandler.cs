using Frank.Abstractions.Errors;
using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.ExceptionHandling;

[AutoRegister(ServiceLifetime.Singleton)]
public interface IExceptionHandler
{
    bool CanHandle(Exception exception);

    IErrorCode GetErrorCode(Exception exception);

    ProblemDetails CreateProblemDetails(Exception exception);
}
