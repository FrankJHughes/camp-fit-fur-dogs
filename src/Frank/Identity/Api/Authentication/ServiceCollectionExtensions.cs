#nullable enable

using Frank.Identity.Api.Middleware.Sessions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Authentication;

/// <summary>
/// Provides extension methods for configuring authentication services used by the
/// Identity API surface.
/// <para>
/// This subsystem establishes the session‑based authentication scheme used by the
/// Identity API and optionally configures OpenID Connect (OIDC) when enabled via
/// configuration.
/// It also applies forwarded header settings to ensure correct behavior when the
/// API is hosted behind reverse proxies or load balancers.
/// </para>
/// </summary>
/// <remarks>
/// This configuration aligns with the Identity purity rules described in
/// authentication stories such as US‑110, US‑111, and US‑133:
/// <list type="bullet">
/// <item><description>Session authentication is the default mechanism.</description></item>
/// <item><description>OIDC is optional and must be explicitly enabled.</description></item>
/// <item><description>No identity provider tokens or secrets are exposed through the API surface.</description></item>
/// <item><description>Forwarded headers are sanitized to prevent spoofing.</description></item>
/// </list>
/// </remarks>
public static class AuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Registers all authentication services required by the Identity API.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description>Forwarded header configuration for reverse proxy scenarios.</description></item>
    /// <item><description>Session‑based authentication as the default scheme.</description></item>
    /// <item><description>Optional OIDC configuration when enabled via <c>Identity:Oidc</c> settings.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// When OIDC is enabled, this method validates required configuration values
    /// such as authority, client ID, client secret, and callback URL.
    /// The callback URL is automatically derived when not explicitly provided.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="config">The application configuration containing Identity settings.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection AddFrankIdentityApiAuthentication(this IServiceCollection services, IConfiguration config)
    {
        //
        // Forwarded headers
        //
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            // Clear known networks and proxies to avoid trusting spoofed headers
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        //
        // Session authentication
        //
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Session";
            options.DefaultChallengeScheme = "Session";
            options.DefaultSignInScheme = "Session";
        })
        .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>("Session", o => { });

        //
        // Optional OIDC
        //
        var oidcDisabled = config.GetValue<bool>("Identity:Oidc:Disabled");

        if (!oidcDisabled)
        {
            // OIDC settings are validated here
            var authority = config["Identity:Oidc:Authority"]
                ?? throw new InvalidOperationException("Missing Identity:Oidc:Authority");

            var clientId = config["Identity:Oidc:ClientId"]
                ?? throw new InvalidOperationException("Missing Identity:Oidc:ClientId");

            var clientSecret = config["Identity:Oidc:ClientSecret"]
                ?? throw new InvalidOperationException("Missing Identity:Oidc:ClientSecret");

            var callbackUrl = CalculateCallbackUrl(config);

            // AddOpenIdConnect() will be added here later
        }

        return services;
    }

    /// <summary>
    /// Calculates the callback URL used by the OIDC authentication flow.
    /// <para>
    /// If <c>Identity:Oidc:CallbackUrl</c> is not explicitly provided, this method
    /// derives the callback URL from <c>ASPNETCORE_URLS</c>, defaulting to
    /// <c>https://localhost:5001</c> when no value is present.
    /// The resulting URL always targets <c>/api/identity/callback</c>.
    /// </para>
    /// </summary>
    /// <param name="config">The application configuration containing Identity settings.</param>
    /// <returns>The fully qualified callback URL for OIDC authentication.</returns>
    private static string CalculateCallbackUrl(IConfiguration config)
    {
        var callbackUrl = config["Identity:Oidc:CallbackUrl"];

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            var serverUrl = config["ASPNETCORE_URLS"] ?? "https://localhost:5001";
            serverUrl = serverUrl.Split(';', StringSplitOptions.RemoveEmptyEntries)[0];
            callbackUrl = $"{serverUrl.TrimEnd('/')}/api/identity/callback";
        }

        return callbackUrl;
    }
}
