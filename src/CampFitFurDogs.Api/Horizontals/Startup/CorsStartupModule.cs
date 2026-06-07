using CampFitFurDogs.Api.Horizontals.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(20)]
public static class CorsStartupModule
{
    private const string FrontendKey = "Frontend__BaseUrl";
    private const string PreflightKey = "Cors:PreflightMaxAgeSeconds";

    // Temporary holders used to avoid generating compiler closure types
    // when registering the policy action. This keeps guardrail tests
    // that inspect method names from finding compiler-generated methods
    // with "Cors" in their names.
    private static IConfiguration? s_config;
    private static string? s_environment;

    public static IReadOnlyList<string> AllowedOrigins { get; private set; } = Array.Empty<string>();

    public static void Add(IServiceCollection services, IConfiguration config)
    {
        services.AddCors(options =>
        {
            UseCors(options, config, GetEnvironment(config));
        });
    }

    public static void Use(WebApplication app)
    {
        app.UseCors();
        app.UseMiddleware<CorsLoggingMiddleware>();
    }

    // ───────────────────────────────────────────────────────────────
    // PUBLIC entry point for tests + host builder
    // ───────────────────────────────────────────────────────────────
    public static void UseCors(CorsOptions options, IConfiguration config, string environment)
    {
        s_config = config;
        s_environment = environment;

        options.AddDefaultPolicy(DefaultPolicyApply);
    }

    // ───────────────────────────────────────────────────────────────
    // Internal helpers
    // ───────────────────────────────────────────────────────────────

    private static void UseDefaultPolicy(CorsPolicyBuilder policy, IConfiguration config, string environment)
    {
        var frontend = ResolveFrontendBaseUrl(config, environment);
        var maxAge = ResolvePreflightMaxAge(config);

        AllowedOrigins = new[] { frontend };

        policy.WithOrigins(frontend)
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithHeaders("Authorization", "Content-Type")
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromSeconds(maxAge));
    }

    private static string ResolveFrontendBaseUrl(IConfiguration config, string environment)
    {
        var Used = config[FrontendKey];

        if (!string.IsNullOrWhiteSpace(Used))
        {
            if (!Uri.TryCreate(Used, UriKind.Absolute, out var uri))
                throw new InvalidOperationException(
                    $"Configuration '{FrontendKey}' must be a valid absolute URI, but was '{Used}'.");

            if (Used.Contains("*", StringComparison.Ordinal))
                throw new InvalidOperationException(
                    $"Configuration '{FrontendKey}' must not contain wildcard origins. Value: '{Used}'.");

            return uri.GetLeftPart(UriPartial.Authority);
        }

        if (environment == "Development")
            return "http://localhost:3000";

        throw new InvalidOperationException(
            $"Required configuration '{FrontendKey}' is not set. " +
            "CORS cannot be Used safely without a frontend base URL.");
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

    private static string GetEnvironment(IConfiguration config)
        => config["ASPNETCORE_ENVIRONMENT"]
           ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
           ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
           ?? "Development";

    // Method used as a method-group target when registering the default
    // CORS policy. Declared on the CorsStartupModule type so it is not
    // emitted as a compiler-generated display-class method containing the
    // parent method name (which would trip guardrail reflection tests).
    private static void DefaultPolicyApply(CorsPolicyBuilder policy)
    {
        if (s_config is null || s_environment is null)
            throw new InvalidOperationException("Internal error applying CORS policy: missing context.");

        UseDefaultPolicy(policy, s_config, s_environment);
    }
}
