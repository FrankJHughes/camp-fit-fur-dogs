using System.Reflection;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Frank.Core.Application.Cqrs.Commands;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankCoreApplicationCqrsCommandDispatcher(this IServiceCollection services)
    {
        services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();
        return services;
    }

    public static IServiceCollection AddFrankCoreApplicationCqrsCommands(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddFrankCoreApplicationCqrsCommandDispatcher();

        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be ICommandHandler<> or ICommandHandler<,>
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            (
                iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            ));

        //
        // Implementations: any class implementing ICommandHandler<> or ICommandHandler<,>
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                (
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                )));

        configure?.Invoke(options);

        IEnumerable<Assembly> assemblies = [
            typeof(Frank.Core.Application.AssemblyMarker).Assembly,
            .. assembliesToSearch
        ];

        Orchestrator.Orchestrate(
            services,
            assemblies,
            options);

        return services;
    }
}
