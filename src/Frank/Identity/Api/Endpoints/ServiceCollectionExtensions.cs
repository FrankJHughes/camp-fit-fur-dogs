#nullable enable

using Frank.Core.Api.Endpoints;
using Frank.Core.Application.Registration;
using Frank.Identity.Api.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Endpoints;

/// <summary>
/// Provides extension methods for registering all Identity API endpoints.
/// <para>
/// This extension configures required settings, applies validation rules, and
/// registers all endpoint implementations discovered within the Identity API
/// assembly.
/// It ensures that endpoint discovery remains explicit, safe, and aligned with
/// DI governance rules such as auto‑registration opt‑out (US‑185).
/// </para>
/// </summary>
/// <remarks>
/// This configuration follows Identity purity and startup rules:
/// <list type="bullet">
/// <item><description>All endpoint settings are validated at startup.</description></item>
/// <item><description>Only endpoints within the Identity API namespace are discovered.</description></item>
/// <item><description>No domain logic is embedded in endpoint registration.</description></item>
/// <item><description>Endpoint discovery is deterministic and governed by <see cref="DiscoveryOptions"/>.</description></item>
/// </list>
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Identity API endpoints and validates required configuration.
    /// <para>
    /// This method performs three key actions:
    /// <list type="bullet">
    /// <item><description>Binds and validates <see cref="FrontendSettings"/>.</description></item>
    /// <item><description>Restricts endpoint discovery to the Identity API namespace.</description></item>
    /// <item><description>Registers all endpoint implementations using Frank Core’s endpoint loader.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Settings validation ensures that dependent endpoints such as
    /// <c>GetLoginUrlEndpoint</c> and <c>LogoutEndpoint</c> fail fast when required
    /// configuration is missing or malformed.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection AddFrankIdentityApiEndpoints(this IServiceCollection services)
    {
        services
            .AddOptions<FrontendSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart(); // Dependents: GetLoginUrlEndpoint, LogoutEndpoint

        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeImplementations(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddFrankCoreApiEndpoints(
                [typeof(AssemblyMarker).Assembly],
                options => updateOptions(options)
            );
    }
}
