using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using SharedKernel.DependencyInjection;
using SharedKernel.Domain;
using SharedKernel.Events;

namespace SharedKernel.DependencyInjection;

public static class SharedKernelServiceCollectionExtensions
{
    /// <summary>
    /// Registers the SharedKernel foundation and product-specific DI rules.
    /// Does NOT reference SharedKernel.Api or perform endpoint discovery.
    /// </summary>
    public static IServiceCollection AddSharedKernel(
        this IServiceCollection services,
        Assembly[] applicationAssemblies,
        Action<SharedKernelOptions>? configure = null)
    {
        // Register CQRS, validation, pipeline, domain events
        services.AddSharedKernelCore(applicationAssemblies);

        // Apply product-specific DI rules
        var options = new SharedKernelOptions();
        configure?.Invoke(options);

        foreach (var registration in options.InfrastructureRegistrations)
        {
            registration(services);
        }

        // NOTE:
        // Endpoint auto-discovery is intentionally NOT invoked here.
        // SharedKernel must not reference SharedKernel.Api.
        // Program.cs is the composition root and performs endpoint registration.

        return services;
    }

    /// <summary>
    /// Registers all Application-layer components:
    /// - Command handlers
    /// - Query handlers
    /// - Pipeline behaviors
    /// - Validators
    /// - Domain event handlers
    /// - Dispatchers
    /// - Shared abstractions
    /// </summary>
    public static IServiceCollection AddSharedKernelCore(
        this IServiceCollection services,
        params Assembly[] applicationAssemblies)
    {
        // COMMAND HANDLERS
        services.Scan(scan => scan
            .FromAssemblies(applicationAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // QUERY HANDLERS
        services.Scan(scan => scan
            .FromAssemblies(applicationAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // // PIPELINE BEHAVIORS
        // services.Scan(scan => scan
        //     .FromAssemblies(applicationAssemblies)
        //     .AddClasses(c => c.AssignableTo(typeof(IPipelineBehavior<,>)))
        //     .AsImplementedInterfaces()
        //     .WithTransientLifetime());

        // VALIDATORS
        services.Scan(scan => scan
            .FromAssemblies(applicationAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        // DOMAIN EVENT HANDLERS
        services.Scan(scan => scan
            .FromAssemblies(applicationAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // SHARED ABSTRACTIONS
        // services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        // services.AddSingleton<IGuidGenerator, GuidGenerator>();

        // CQRS DISPATCHERS
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // DOMAIN EVENT DISPATCHER
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
