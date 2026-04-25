using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Events;

namespace SharedKernel.DependencyInjection;

public static class SharedKernelServiceCollectionExtensions
{
    public static IServiceCollection AddSharedKernel(
        this IServiceCollection services,
        Assembly[] applicationAssemblies,
        Action<SharedKernelOptions>? configure = null)
    {
        services.AddSharedKernelCore(applicationAssemblies);

        var options = new SharedKernelOptions();
        configure?.Invoke(options);

        foreach (var registration in options.InfrastructureRegistrations)
            registration(services);

        return services;
    }

    public static IServiceCollection AddSharedKernelCore(
        this IServiceCollection services,
        params Assembly[] applicationAssemblies)
    {
        if (applicationAssemblies is null || applicationAssemblies.Length == 0)
            throw new InvalidOperationException("No application assemblies provided to AddSharedKernelCore.");

        var allTypes = applicationAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition)
            .ToList();

        RegisterCommandHandlers(services, allTypes);
        RegisterQueryHandlers(services, allTypes);
        RegisterValidators(services, allTypes);
        RegisterDomainEventHandlers(services, allTypes);

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    // ------------------------------------------------------------
    // COMMAND HANDLERS
    // ------------------------------------------------------------
    private static void RegisterCommandHandlers(IServiceCollection services, List<Type> types)
    {
        var handlers = types
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)) &&
                        !i.ContainsGenericParameters)
                    .Select(i => (Service: i, Implementation: t)))
            .ToList();

        // If commands exist but no handlers → fail
        if (handlers.Count == 0)
        {
            var hasCommands = types.Any(t =>
                t.GetInterfaces().Any(i =>
                    (i == typeof(ICommand)) ||
                    (i.IsGenericType &&
                     i.GetGenericTypeDefinition() == typeof(ICommand<>))));

            if (hasCommands)
                throw new InvalidOperationException("No command handlers found during AddSharedKernelCore().");

            return;
        }

        // Duplicate detection by CLOSED generic interface
        var duplicates = handlers
            .GroupBy(h => h.Service)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicates.Any())
        {
            var message = "Duplicate command handlers found for: " +
                          string.Join(", ", duplicates.Select(g => g.Key));
            throw new InvalidOperationException(message);
        }

        foreach (var (service, implementation) in handlers)
        {
            services.AddScoped(service, implementation);
            services.AddScoped(implementation, implementation);
        }
    }

    // ------------------------------------------------------------
    // QUERY HANDLERS
    // ------------------------------------------------------------
    private static void RegisterQueryHandlers(IServiceCollection services, List<Type> types)
    {
        var handlers = types
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) &&
                        !i.ContainsGenericParameters)
                    .Select(i => (Service: i, Implementation: t)))
            .ToList();

        foreach (var (service, implementation) in handlers)
        {
            services.AddScoped(service, implementation);
            services.AddScoped(implementation, implementation);
        }
    }

    // ------------------------------------------------------------
    // VALIDATORS
    // ------------------------------------------------------------
    private static void RegisterValidators(IServiceCollection services, List<Type> types)
    {
        var validators = types
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidator<>) &&
                        !i.ContainsGenericParameters)
                    .Select(i => (Service: i, Implementation: t)))
            .ToList();

        foreach (var (service, implementation) in validators)
        {
            services.AddTransient(service, implementation);
            services.AddTransient(implementation, implementation);
        }
    }

    // ------------------------------------------------------------
    // DOMAIN EVENT HANDLERS
    // ------------------------------------------------------------
    private static void RegisterDomainEventHandlers(IServiceCollection services, List<Type> types)
    {
        var handlers = types
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>) &&
                        !i.ContainsGenericParameters)
                    .Select(i => (Service: i, Implementation: t)))
            .ToList();

        foreach (var (service, implementation) in handlers)
        {
            services.AddScoped(service, implementation);
            services.AddScoped(implementation, implementation);
        }
    }
}
