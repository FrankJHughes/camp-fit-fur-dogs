using Microsoft.Extensions.DependencyInjection;
using Scrutor;

using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.DomainEvents;
using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Application.DependencyInjection;

public static class DomainEventRegistration
{
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        var assembly = typeof(DomainEventRegistration).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
