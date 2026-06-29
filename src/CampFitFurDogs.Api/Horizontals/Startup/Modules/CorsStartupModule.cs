using CampFitFurDogs.Api.Horizontals.Cors.Middleware;
using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(50)]
public sealed class CorsStartupModule : IStartupModule
{
    private const string FrontendKey = "Frontend:BaseUrl";
    private const string OidcAuthorityKey = "Authentication:Callback:Oidc:Authority";
    private const string PreflightKey = "Cors:PreflightMaxAgeSeconds";

    private string? _frontendOrigin;
    private string? _oidcOrigin;
    private int _preflightSeconds;

    public IReadOnlyList<string> AllowedOrigins { get; private set; } = Array.Empty<string>();

    public void Add(WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var environment = ResolveEnvironment(config);

        _frontendOrigin = ResolveOrigin(config, FrontendKey, environmentFallback: "http://localhost:3000", environment);
        _oidcOrigin = ResolveOrigin(config, OidcAuthorityKey, environmentFallback: null, environment);

        _preflightSeconds = ResolvePreflightMaxAge(config);

        AllowedOrigins = new[]
        {
            _frontendOrigin!,
            _oidcOrigin!
        };

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(ApplyDefaultPolicy);
        });
    }

    public void Use(WebApplication app)
    {
        app.UseRouting();
        app.UseCors();
        app.UseOriginLogging();
    }

    private void ApplyDefaultPolicy(CorsPolicyBuilder policy)
    {
        if (_frontendOrigin is null || _oidcOrigin is null)
            throw new InvalidOperationException("CORS policy applied before Add() initialized configuration.");

        policy
            .WithOrigins(_frontendOrigin, _oidcOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(_preflightSeconds));
    }

    private static string ResolveOrigin(IConfiguration config, string key, string? environmentFallback, string environment)
    {
        var raw = config[key];

        if (!string.IsNullOrWhiteSpace(raw))
        {
            if (raw.Contains("*", StringComparison.Ordinal))
                throw new InvalidOperationException(
                    $"Configuration '{key}' must not contain wildcard origins. Value: '{raw}'.");

            if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
                throw new InvalidOperationException(
                    $"Configuration '{key}' must be a valid absolute URI, but was '{raw}'.");

            return uri.IsDefaultPort
                ? $"{uri.Scheme}://{uri.Host}"
                : $"{uri.Scheme}://{uri.Host}:{uri.Port}";
        }

        if (environment == "Development" && environmentFallback is not null)
            return environmentFallback;

        throw new InvalidOperationException(
            $"Required configuration '{key}' is not set. CORS cannot be configured safely without it.");
    }

    private static int ResolvePreflightMaxAge(IConfiguration config)
    {
        var raw = config[PreflightKey];

        if (string.IsNullOrWhiteSpace(raw))
            return 3600;

        if (!int.TryParse(raw, out var seconds))
            throw new InvalidOperationException(
                $"Configuration '{PreflightKey}' must be an integer number of seconds.");

        if (seconds <= 0 || seconds > 86400)
            throw new InvalidOperationException(
                $"Configuration '{PreflightKey}' must be between 1 and 86400 seconds.");

        return seconds;
    }

    private static string ResolveEnvironment(IConfiguration config)
        => config["ASPNETCORE_ENVIRONMENT"]
           ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
           ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
           ?? "Development";
}
