using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApplication(this IServiceCollection services)
    {
        return services
            .AddFrankCoreApplicationCqrsCommandDispatcher()
            .AddFrankCoreApplicationCqrsQueryDispatcher()
            .AddFrankCoreApplicationDomainEventDispatcher()
            ;
    }
}
