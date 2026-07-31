using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform.Cors;

public static class CorsServiceCollectionExtensions
{
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
