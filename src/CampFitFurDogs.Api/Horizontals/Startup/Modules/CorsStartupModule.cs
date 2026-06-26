using CampFitFurDogs.Api.Horizontals.Cors;
using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(50)]
public sealed class CorsStartupModule : IStartupModule
{
    private const string FrontendKey = "Frontend:BaseUrl";
    private const string PreflightKey = "Cors:PreflightMaxAgeSeconds";

    private string? _frontendOrigin;
    private int _preflightSeconds;

    public IReadOnlyList<string> AllowedOrigins { get; private set; } = Array.Empty<string>();

    // ---------------------------------------------------------------------
    // ADD PHASE — VALIDATION MUST HAPPEN HERE
    // ---------------------------------------------------------------------
    public void Add(WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var environment = ResolveEnvironment(config);

        _frontendOrigin = ResolveFrontendOrigin(config, environment);
        _preflightSeconds = ResolvePreflightMaxAge(config);

        AllowedOrigins = new[] { _frontendOrigin };

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(ApplyDefaultPolicy);
        });
    }

    // ---------------------------------------------------------------------
    // USE PHASE
    // ---------------------------------------------------------------------
    public void Use(WebApplication app)
    {
        app.UseCors();
        app.UseMiddleware<CorsLoggingMiddleware>();
    }

    // ---------------------------------------------------------------------
    // APPLY POLICY — NO VALIDATION HERE
    // ---------------------------------------------------------------------
    private void ApplyDefaultPolicy(CorsPolicyBuilder policy)
    {
        if (_frontendOrigin is null)
            throw new InvalidOperationException("CORS policy applied before Add() initialized configuration.");

        policy
            .WithOrigins(_frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(_preflightSeconds));
    }

    // ---------------------------------------------------------------------
    // VALIDATION HELPERS
    // ---------------------------------------------------------------------
    private static string ResolveFrontendOrigin(IConfiguration config, string environment)
    {
        var raw = config[FrontendKey];

        if (!string.IsNullOrWhiteSpace(raw))
        {
            if (raw.Contains("*", StringComparison.Ordinal))
                throw new InvalidOperationException(
                    $"Configuration '{FrontendKey}' must not contain wildcard origins. Value: '{raw}'.");

            if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
                throw new InvalidOperationException(
                    $"Configuration '{FrontendKey}' must be a valid absolute URI, but was '{raw}'.");

            return uri.IsDefaultPort
                ? $"{uri.Scheme}://{uri.Host}"
                : $"{uri.Scheme}://{uri.Host}:{uri.Port}";
        }

        if (environment == "Development")
            return "http://localhost:3000";

        throw new InvalidOperationException(
            $"Required configuration '{FrontendKey}' is not set. " +
            "CORS cannot be configured safely without a frontend base URL.");
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
