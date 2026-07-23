using Microsoft.Extensions.DependencyInjection;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Infrastructure.Clock;

namespace Frank.Core.Infrastructure.Observations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreInfrastructureObservations(this IServiceCollection services)
    {
        return services

            .AddSingleton<IObservationSink, ObservationSink>()
            .AddSingleton<IMetrics, Metrics>()
            .AddSingleton<ICorrelationContext, CorrelationContext>()
            .AddSingleton<IErrorBoundaryObserver, ErrorBoundaryObserver>()

            // REQUIRED for ObservationContext constructors
            .AddFrankCoreInfrastructureClock()

            // Factory for creating SystemObservationContext
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
