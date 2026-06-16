using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(60)]
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

        var auth = services
            .AddAuthentication(options =>
            {
                // ⭐ These two are critical
                options.DefaultScheme = "cfd.session";
                options.DefaultAuthenticateScheme = "cfd.session";
                options.DefaultChallengeScheme = disableOidc
                    ? "cfd.session"
                    : OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie("cfd.session", options =>
            {
                var env = builder.Environment;

                options.Cookie.Name = "cfd.session";
                options.Cookie.Path = "/";
                options.Cookie.HttpOnly = true;

                options.Cookie.SecurePolicy = env.IsProduction()
                    ? CookieSecurePolicy.Always
                    : CookieSecurePolicy.None;

                // OIDC requires SameSite=None
                options.Cookie.SameSite = SameSiteMode.None;

                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";
            });

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
            });
        }
    }

    public void Use(WebApplication app)
    {
        app.UseAuthentication();
    }
}
