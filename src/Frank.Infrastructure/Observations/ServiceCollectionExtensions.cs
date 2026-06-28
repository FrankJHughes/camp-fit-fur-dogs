using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Frank.Abstractions.Observations;
using Frank.Infrastructure.Observations.Http;

namespace Frank.Infrastructure.Observations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankObservations(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<IObservationSink, ObservationSink>();
        services.AddSingleton<IMetrics, Metrics>();
        services.AddSingleton<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<IErrorBoundaryObserver, ErrorBoundaryObserver>();

        // Make ObservabilityContext available per-request
        services.AddScoped<IRequestObservationContext>(provider =>
        {
            var http = provider.GetRequiredService<IHttpContextAccessor>();
            var context = http.HttpContext?.Items[nameof(IRequestObservationContext)]
                as IRequestObservationContext;

            if (context is not null)
                return context;

            // Fallback for startup/test/background contexts
            var env = provider.GetRequiredService<IHostEnvironment>();
            return new DefaultRequestObservationContext(env);
        });

        services.AddTransient<OutboundObservationContextHandler>();

        services.AddHttpClient("*")
                .AddHttpMessageHandler<OutboundObservationContextHandler>();

        return services;
    }
}
