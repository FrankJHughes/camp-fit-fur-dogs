#nullable enable
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Middleware.Observations;

/// <summary>
/// Provides extension methods for registering the observation‑related services
/// used by the Frank.Core API.
/// <para>
/// This includes wiring up outbound HTTP observation propagation via
/// <see cref="OutboundObservationContextHandler"/> and ensuring that
/// <see cref="IHttpContextAccessor"/> is available for inbound observation
/// context creation.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the observation middleware components required for propagating
    /// correlation identifiers, channel metadata, agent metadata, and W3C
    /// TraceContext headers on outbound HTTP requests.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>
    /// Adds a named <c>*</c> <see cref="HttpClient"/> that automatically applies
    /// <see cref="OutboundObservationContextHandler"/> to all outbound requests.
    /// </description></item>
    /// <item><description>
    /// Registers <see cref="OutboundObservationContextHandler"/> as a transient
    /// dependency so it can be resolved per request using the current
    /// <see cref="IRequestObservationContext"/>.
    /// </description></item>
    /// <item><description>
    /// Adds <see cref="IHttpContextAccessor"/> to support inbound observation
    /// context creation.
    /// </description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which observation services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// configuration.
    /// </returns>
    public static IServiceCollection AddFrankCoreApiObservations(this IServiceCollection services)
    {
        _ = services
            .AddHttpClient("*")
            .AddHttpMessageHandler<OutboundObservationContextHandler>();

        return services
            .AddHttpContextAccessor()
            .AddTransient<OutboundObservationContextHandler>();
    }
}
