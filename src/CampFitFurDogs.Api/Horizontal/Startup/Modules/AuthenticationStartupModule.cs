using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;

namespace CampFitFurDogs.Api.Horizontal.Startup.Modules;

[StartupModule(70)]
public class AuthenticationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        var env = builder.Environment;
        var services = builder.Services;
        var config = builder.Configuration;

        var oidcDisabled = config.GetValue<bool>("Authentication:Callback:Oidc:Disabled");

        var auth = services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "cfd.session";
                options.DefaultAuthenticateScheme = "cfd.session";
                options.DefaultChallengeScheme = oidcDisabled
                    ? "cfd.session"
                    : OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie("cfd.session", options =>
            {
                options.Cookie.Name = "cfd.session";
                options.Cookie.Path = "/";
                options.Cookie.HttpOnly = true;

                options.Cookie.SecurePolicy = env.IsProduction()
                    ? CookieSecurePolicy.Always
                    : CookieSecurePolicy.None;

                // Required for OIDC
                options.Cookie.SameSite = SameSiteMode.None;

                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";

                //
                // Prevent 302 redirects for APIs.
                //
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };
            });

        if (!oidcDisabled)
        {
            var authority = config["Authentication:Callback:Oidc:Authority"];
            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new InvalidOperationException("Missing Authentication:Callback:Oidc:Authority");
            }

            var clientId = config["Authentication:Callback:Oidc:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException("Missing Authentication:Callback:Oidc:ClientId");
            }

            var clientSecret = config["Authentication:Callback:Oidc:ClientSecret"];
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException("Missing Authentication:Callback:Oidc:ClientSecret");
            }

            var postLoginRedirectUrl = config["Authentication:Callback:PostLoginRedirectUrl"];
            if (string.IsNullOrWhiteSpace(postLoginRedirectUrl))
            {
                throw new InvalidOperationException("Missing Authentication:Callback:PostLoginRedirectUrl");
            }

            string callbackUrl = CalculateCallbackUrl(config);
            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                throw new InvalidOperationException("Missing Authentication:Callback:Oidc:CallbackUrl or incorrect ASPNETCORE_URLS");
            }

            auth.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authority;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;

                options.CallbackPath = new PathString(new Uri(callbackUrl).AbsolutePath);
                options.ResponseType = "code";
                options.SaveTokens = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    var req = context.Request;

                    context.ProtocolMessage.RedirectUri =
                        $"{req.Scheme}://{req.Host}/api/auth/callback";

                    return Task.CompletedTask;
                };

                // You may add events here later for claim mapping, etc.
            });
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
            callbackUrl = $"{serverUrl.TrimEnd('/')}/api/auth/callback";
        }

        return callbackUrl;
    }

    public void Use(WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseAuthentication();
    }
}
