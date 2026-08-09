using Frank.Core.Application.Abstractions.Clock;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Clock;

/// <summary>
/// Provides DI registration extensions for the infrastructure‑level clock
/// implementation.
/// <para>
/// This module registers <see cref="SystemClock"/> as the default
/// <see cref="IClock"/> implementation, supplying the system UTC time for
/// production scenarios. Vertical slices or test environments may override
/// this registration with custom clock implementations when deterministic
/// time behavior is required.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the infrastructure clock (<see cref="SystemClock"/>) as the
    /// scoped implementation of <see cref="IClock"/>.
    /// </summary>
    /// <param name="services">
    /// The service collection to modify.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection AddFrankCoreInfrastructureClock(this IServiceCollection services)
    {
        return services
            .AddScoped<IClock, SystemClock>();
    }
}
