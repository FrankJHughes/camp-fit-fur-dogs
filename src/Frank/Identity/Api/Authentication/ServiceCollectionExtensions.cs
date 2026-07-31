using Frank.Identity.Api.Middleware.Sessions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
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

            // AddOpenIdConnect() goes here later
        }

        return services;
    }

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
