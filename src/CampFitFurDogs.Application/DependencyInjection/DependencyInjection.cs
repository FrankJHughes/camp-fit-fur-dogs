using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.DomainEvents;

namespace CampFitFurDogs.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Handlers + Validators via Scrutor
        services.Scan(scan => scan
            .FromAssemblies(assembly)

            // Command Handlers
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()

            // Query Handlers
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()

            // Validators
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                .AsSelf()
                .AsImplementedInterfaces()
                .WithTransientLifetime()

            // Domain Event Handlers
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

        // Dispatchers (intentionally interface-only)
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
