using Frank.Core.Application.Abstractions.Startup;
using Frank.Identity.Api.Middleware.Sessions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;

namespace CampFitFurDogs.Api.StartupModules;

[StartupModule(70)]
public class AuthenticationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        //
        // Forwarded headers (unchanged)
        //
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        var env = builder.Environment;
        var config = builder.Configuration;

        var oidcDisabled = config.GetValue<bool>("Identity:Oidc:Disabled");

        var auth = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Session";
            options.DefaultChallengeScheme = "Session";
            options.DefaultSignInScheme = "Session";
        })
        .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>("Session", o => { });

        //
        // Add OIDC only if enabled
        //
        if (!oidcDisabled)
        {
            var authority = config["Identity:Oidc:Authority"]
                ?? throw new InvalidOperationException("Missing Identity:Oidc:Authority");

            var clientId = config["Identity:Oidc:ClientId"]
                ?? throw new InvalidOperationException("Missing Identity:Oidc:ClientId");

            var clientSecret = config["Identity:Oidc:ClientSecret"]
                ?? throw new InvalidOperationException("Missing Identity:Oidc:ClientSecret");

            var postLoginRedirectUrl = config["Identity:Callback:PostLoginRedirectUrl"]
                ?? throw new InvalidOperationException("Missing Identity:Callback:PostLoginRedirectUrl");

            string callbackUrl = CalculateCallbackUrl(config)
                ?? throw new InvalidOperationException("Missing Identity:Oidc:CallbackUrl or incorrect ASPNETCORE_URLS");
        }
    }

    private static string CalculateCallbackUrl(ConfigurationManager config)
    {
        var callbackUrl = config["Identity:Oidc:CallbackUrl"];

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            var serverUrl = config["ASPNETCORE_URLS"]
                ?? "https://localhost:5001";

            serverUrl = serverUrl.Split(';', StringSplitOptions.RemoveEmptyEntries)[0];
            callbackUrl = $"{serverUrl.TrimEnd('/')}/api/identity/callback";
        }

        return callbackUrl;
    }

    public void Use(WebApplication app)
    {
        app.UseForwardedHeaders();
    }
}
