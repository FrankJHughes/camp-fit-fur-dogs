using System.Reflection;
using Frank.Abstractions.Exceptions;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.Exceptions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankException(this IServiceCollection services)
    {
        return AddFrankException(services, []);
    }

    public static IServiceCollection AddFrankException(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<RegistrationOptions>? configure = null)
    {
        services.AddSingleton<ExceptionHandlerRegistry>();

        var includeInterfaceTypes = new Type[]
        {
            typeof(IExceptionHandler)
        };

        var registrationOptions = new RegistrationOptions();
        configure?.Invoke(registrationOptions);

        Orchestrator.Orchestrate(
            services,
            assemblies,
            includeInterfaceTypes,
            registrationOptions);

        return services;
    }
}
