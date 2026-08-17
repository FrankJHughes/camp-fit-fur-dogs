using Microsoft.Extensions.DependencyInjection;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Infrastructure.Clock;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Provides DI registration extensions for the observability subsystem,
/// including sinks, metrics, correlation IDs, error observers, and both
/// request‑scope and system‑scope observation contexts.
/// <para>
/// This module establishes the unified observability pipeline used across
/// trace events, metrics, logs, and distributed diagnostics.
/// It wires up all infrastructure‑level components required to construct
/// <see cref="IObservationContext"/> instances.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all infrastructure‑level observability services, including
    /// sinks, metrics, correlation ID providers, error observers, and context
    /// factories.
    /// <para>
    /// This method must be called once during application startup to ensure
    /// consistent observability behavior across the system.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddFrankCoreInfrastructureObservations(this IServiceCollection services)
    {
        return services

            // Core observability primitives
            .AddSingleton<IObservationSink, ObservationSink>()
            .AddSingleton<IMetrics, Metrics>()
            .AddSingleton<ICorrelationContext, CorrelationContext>()
            .AddSingleton<IErrorBoundaryObserver, ErrorBoundaryObserver>()

            // Required for ObservationContext constructors
            .AddFrankCoreInfrastructureClock()

            // Factory for creating system-scope observation contexts
            .AddTransient<Func<string, string, IObservationContext>>(sp =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                var clock = sp.GetRequiredService<IClock>();

                return (channel, agent) =>
                    SystemObservationContext.Create(
                        channel: channel,
                        agent: agent,
                        environment: env,
                        clock: clock
                    );
            })

            // Per-request observation context
            .AddScoped<IRequestObservationContext>(provider =>
            {
                var http = provider.GetRequiredService<IHttpContextAccessor>();
                var context = http.HttpContext?.Items[nameof(IRequestObservationContext)]
                    as IRequestObservationContext;

                if (context is not null)
                    return context;

                // Fallback for startup/test/background contexts
                var env = provider.GetRequiredService<IHostEnvironment>();
                var clock = provider.GetRequiredService<IClock>();

                return new DefaultRequestObservationContext(env, clock);
            });
    }
}
