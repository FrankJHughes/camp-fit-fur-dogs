using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Frank.Abstractions.Observability;
using Frank.Infrastructure.Observability.Http;

namespace Frank.Infrastructure.Observability;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankObservability(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<IObservabilitySink, ObservabilitySink>();
        services.AddSingleton<IMetrics, Metrics>();
        services.AddSingleton<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<IErrorBoundaryObserver, ErrorBoundaryObserver>();

        // Make ObservabilityContext available per-request
        services.AddScoped<IRequestObservabilityContext>(provider =>
        {
            var http = provider.GetRequiredService<IHttpContextAccessor>();
            var context = http.HttpContext?.Items[nameof(IRequestObservabilityContext)]
                as IRequestObservabilityContext;

            if (context is not null)
                return context;

            // Fallback for startup/test/background contexts
            var env = provider.GetRequiredService<IHostEnvironment>();
            return new DefaultRequestObservabilityContext(env);
        });

        services.AddTransient<OutboundTraceContextHandler>();

        services.AddHttpClient("*")
                .AddHttpMessageHandler<OutboundTraceContextHandler>();

        return services;
    }
}
