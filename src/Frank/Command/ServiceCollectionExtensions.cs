using System.Reflection;
using Frank.Abstractions.Command;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Command;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCommand(this IServiceCollection services)
    {
        return AddFrankCommand(services, []);
    }

    public static IServiceCollection AddFrankCommand(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        var types = new Type[]
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>)
        };

        Orchestrator.Orchestrate(services, types, assemblies);

        return services;
    }
}
