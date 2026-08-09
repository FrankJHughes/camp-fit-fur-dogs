using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application;

/// <summary>
/// Provides extension methods for registering the core Frank application services
/// into an <see cref="IServiceCollection"/>.
///
/// <para>
/// This method bundles the registration of the following subsystems:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///     <see cref="ICommandDispatcher"/> — CQRS command dispatching
///     </description>
///   </item>
///   <item>
///     <description>
///     <see cref="IQueryDispatcher"/> — CQRS query dispatching
///     </description>
///   </item>
///   <item>
///     <description>
///     <see cref="IEventDispatcher"/> — domain event dispatching
///     </description>
///   </item>
/// </list>
///
/// <para>
/// By convention, application layers call this method during startup to ensure
/// that all core Frank application orchestration components are available.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core Frank application services, including CQRS command
    /// dispatching, CQRS query dispatching, and domain event dispatching.
    ///
    /// <para>
    /// This method is intended to be the primary entry point for configuring
    /// Frank.Core.Application within a host application's dependency injection
    /// container.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the Frank application services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankCoreApplication(this IServiceCollection services)
    {
        return services
            .AddFrankCoreApplicationCqrsCommandDispatcher()
            .AddFrankCoreApplicationCqrsQueryDispatcher()
            .AddFrankCoreApplicationDomainEventDispatcher();
    }
}
