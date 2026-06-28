using System.Reflection;
using Frank.Abstractions.Command;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Command;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankCommand(this IServiceCollection services)
    {
        return AddFrankCommand(services, []);
    }

    public static IServiceCollection AddFrankCommand(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be ICommandHandler<> or ICommandHandler<,>
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterface(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            (
                iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            ));

        //
        // Implementations: any class implementing ICommandHandler<> or ICommandHandler<,>
        //
        options.IncludeImplementation(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                (
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                )));

        configure?.Invoke(options);

        Orchestrator.Orchestrate(
            services,
            [typeof(Frank.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
