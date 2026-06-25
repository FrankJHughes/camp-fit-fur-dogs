using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(70)]
public class AuthenticationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var env = builder.Environment;
        var services = builder.Services;
        var config = builder.Configuration;

        var authority = config["Authentication:Callback:Oidc:Authority"]
            ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:Authority");
        var clientId = config["Authentication:Callback:Oidc:ClientId"]
            ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:ClientId");
        var clientSecret = config["Authentication:Callback:Oidc:ClientSecret"]
            ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:ClientSecret");
        var callbackUrl = config["Authentication:Callback:Oidc:CallbackUrl"]
            ?? throw new InvalidOperationException("Missing Authentication:Callback:Oidc:CallbackUrl");
        var postLoginRedirectUrl = config["Authentication:Callback:PostLoginRedirectUrl"]
            ?? throw new InvalidOperationException("Missing Authentication:Callback:PostLoginRedirectUrl");

        var disableOidc = config.GetValue<bool>("Authentication:Callback:Oidc:Disabled");

        //
        // ⭐ Authentication configuration
        //
        var auth = services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "cfd.session";
                options.DefaultAuthenticateScheme = "cfd.session";
                options.DefaultChallengeScheme = disableOidc
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

        //
        // ⭐ Optional OIDC (disabled in local dev)
        //
        if (!disableOidc)
        {
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

                // You may add events here later for claim mapping, etc.
            });
        }
    }

    public void Use(WebApplication app)
    {
        app.UseAuthentication();
    }
}
