using Frank.Integration.ExceptionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        // IExceptionHandler implementations are auto-registered via [AutoRegister] on the interface.
        services.AddSingleton<ExceptionHandlerRegistry>();
        return services;
    }
}
