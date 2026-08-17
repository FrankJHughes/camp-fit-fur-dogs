#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform.Cors;

/// <summary>
/// Provides extension methods for configuring the Frank.Core API's platform-level
/// Cross-Origin Resource Sharing (CORS) policy.
/// <para>
/// This subsystem resolves and normalizes origins from configuration, validates
/// them, and applies a hardened default CORS policy suitable for frontend and
/// identity flows.
/// It ensures that CORS behavior is environment-driven, predictable, and
/// consistently applied across the entire API surface.
/// </para>
/// </summary>
public static class CorsServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Frank.Core API's default CORS policy using origins resolved
    /// from configuration.
    /// <para>
    /// The method:
    /// <list type="bullet">
    /// <item><description>Resolves and normalizes the frontend origin.</description></item>
    /// <item><description>Resolves and normalizes the identity/OIDC authority origin.</description></item>
    /// <item><description>Validates and applies a configurable preflight max-age.</description></item>
    /// <item><description>Configures a default CORS policy allowing headers, methods, and credentials.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="config">The application configuration used to resolve origins and settings.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection AddFrankCoreApiPlatformCors(this IServiceCollection services, IConfiguration config)
    {
        var frontendOrigin = ResolveOrigin(config, "Frontend:BaseUrl", "http://localhost:3000");
        var oidcOrigin = ResolveOrigin(config, "Identity:Oidc:Authority", null);

        var preflightSeconds = ResolvePreflightMaxAge(config);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins(frontendOrigin, oidcOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(preflightSeconds));
            });
        });

        return services;
    }

    /// <summary>
    /// Resolves and normalizes an origin from configuration.
    /// <para>
    /// If the configuration value exists, it must be a valid absolute URI.
    /// The origin is normalized into canonical form:
    /// <c>scheme://host</c> or <c>scheme://host:port</c> when non-default.
    /// </para>
    /// <para>
    /// If the configuration value is missing and a fallback is provided, the
    /// fallback is returned.
    /// If no fallback exists, an exception is thrown.
    /// </para>
    /// </summary>
    /// <param name="config">The configuration source.</param>
    /// <param name="key">The configuration key to resolve.</param>
    /// <param name="fallback">Optional fallback origin.</param>
    /// <returns>A normalized origin string.</returns>
    private static string ResolveOrigin(IConfiguration config, string key, string? fallback)
    {
        var raw = config[key];

        if (!string.IsNullOrWhiteSpace(raw))
        {
            if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
                throw new InvalidOperationException($"Invalid URI for '{key}': '{raw}'.");

            return uri.IsDefaultPort
                ? $"{uri.Scheme}://{uri.Host}"
                : $"{uri.Scheme}://{uri.Host}:{uri.Port}";
        }

        if (fallback is not null)
            return fallback;

        throw new InvalidOperationException($"Missing required configuration '{key}'.");
    }

    /// <summary>
    /// Resolves the CORS preflight max-age value from configuration.
    /// <para>
    /// If the value is missing, a default of <c>3600</c> seconds is used.
    /// The value must parse as an integer and fall within the inclusive range
    /// <c>1</c> to <c>86400</c>.
    /// Invalid or out-of-range values result in descriptive exceptions.
    /// </para>
    /// </summary>
    /// <param name="config">The configuration source.</param>
    /// <returns>The validated preflight max-age in seconds.</returns>
    private static int ResolvePreflightMaxAge(IConfiguration config)
    {
        var raw = config["Cors:PreflightMaxAgeSeconds"];

        if (string.IsNullOrWhiteSpace(raw))
            return 3600;

        if (!int.TryParse(raw, out var seconds))
            throw new InvalidOperationException($"Invalid preflight max age: '{raw}'.");

        if (seconds <= 0 || seconds > 86400)
            throw new InvalidOperationException("Preflight max age must be between 1 and 86400 seconds.");

        return seconds;
    }
}
