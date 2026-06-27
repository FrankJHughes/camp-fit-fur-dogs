using System.Reflection;
using Frank.Abstractions.Problem;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.Problem;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankProblem(this IServiceCollection services)
    {
        return AddFrankProblem(services, []);
    }

    public static IServiceCollection AddFrankProblem(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<ExceptionHandlerRegistry>();

        var types = new Type[]
        {
            typeof(IExceptionHandler)
        };

        Orchestrator.Orchestrate(services, types, assemblies);

        return services;
    }
}
