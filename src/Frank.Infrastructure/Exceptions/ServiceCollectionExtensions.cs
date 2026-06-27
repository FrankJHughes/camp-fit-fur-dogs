using System.Reflection;
using Frank.Abstractions.Exceptions;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.Exceptions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankProblem(this IServiceCollection services)
    {
        return AddFrankProblem(services, []);
    }

    public static IServiceCollection AddFrankProblem(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<ExceptionHandlerRegistry>();

        var includeInterfaceTypes = new Type[]
        {
            typeof(IExceptionHandler)
        };

        var excludeConcreteTypes = Array.Empty<Type>();

        Orchestrator.Orchestrate(
            services,
            includeInterfaceTypes,
            excludeConcreteTypes,
            assemblies);

        return services;
    }
}
