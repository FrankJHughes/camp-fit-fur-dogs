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

    public static IServiceCollection AddFrankCommand(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<RegistrationOptions>? configure = null)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        var includeInterfaceTypes = new Type[]
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>)
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
