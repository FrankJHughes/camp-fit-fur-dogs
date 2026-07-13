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

        var oidcDisabled = config.GetValue<bool>("Authentication:Callback:Oidc:Disabled");

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
            var authority = config["Authentication:Callback:Oidc:Authority"]
                ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:Authority");

            var clientId = config["Authentication:Callback:Oidc:ClientId"]
                ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:ClientId");

            var clientSecret = config["Authentication:Callback:Oidc:ClientSecret"]
                ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:ClientSecret");

            var postLoginRedirectUrl = config["Authentication:Callback:PostLoginRedirectUrl"]
                ?? throw new InvalidOperationException("Missing Authentication:Callback:PostLoginRedirectUrl");

            string callbackUrl = CalculateCallbackUrl(config)
                ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:CallbackUrl or incorrect ASPNETCORE_URLS");

            // auth.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            // {
            //     options.Authority = authority;
            //     options.ClientId = clientId;
            //     options.ClientSecret = clientSecret;

            //     options.CallbackPath = new PathString(new Uri(callbackUrl).AbsolutePath);
            //     options.ResponseType = "code";
            //     options.SaveTokens = true;

            //     options.Scope.Clear();
            //     options.Scope.Add("openid");
            //     options.Scope.Add("profile");
            //     options.Scope.Add("email");

            //     //
            //     // FIX: Only redirect to OIDC when explicitly invoked.
            //     //
            //     options.Events.OnRedirectToIdentityProvider = context =>
            //     {
            //         var req = context.Request;

            //         context.ProtocolMessage.RedirectUri =
            //             $"{req.Scheme}://{req.Host}/api/identity/callback";

            //         return Task.CompletedTask;
            //     };
            // });
        }
    }

    private static string CalculateCallbackUrl(ConfigurationManager config)
    {
        var callbackUrl = config["Authentication:Callback:Oidc:CallbackUrl"];

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
