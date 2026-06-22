using Frank.Abstractions.Observability;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    /// <summary>
    /// Registers the core observability infrastructure services.
    /// This provides the default no-op implementations required by US-197.
    /// </summary>
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddSingleton<ITraceEvents, TraceEvents>();
        services.AddSingleton<IMetrics, Metrics>();
        services.AddSingleton<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<IErrorBoundaryObserver, ErrorBoundaryObserver>();

        // ObservabilityContext is created per operation/request,
        // so we register a factory instead of a singleton.
        services.AddTransient<IObservabilityContext>(_ =>
            new ObservabilityContext());

        return services;
    }
}
