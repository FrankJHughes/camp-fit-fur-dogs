using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CampFitFurDogs.Api.Configuration;

[Configurator(20)]
public static class AuthenticationConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        //
        // Read from appsettings.json structure:
        // "Authentication": {
        //   "Oidc": {
        //     "Authority": "...",
        //     "ClientId": "...",
        //     "ClientSecret": "...",
        //     "CallbackUrl": "...",
        //     "PostLoginRedirectUrl": "..."
        //   }
        // }
        //

        var authority = config["Authentication:Oidc:Authority"]
            ?? throw new InvalidOperationException("Missing configuration: Authentication:Oidc:Authority");

        var clientId = config["Authentication:Oidc:ClientId"]
            ?? throw new InvalidOperationException("Missing configuration: Authentication:Oidc:ClientId");

        var clientSecret = config["Authentication:Oidc:ClientSecret"]
            ?? throw new InvalidOperationException("Missing configuration: Authentication:Oidc:ClientSecret");

        var callbackUrl = config["Authentication:Oidc:CallbackUrl"]
            ?? throw new InvalidOperationException("Missing configuration: Authentication:Oidc:CallbackUrl");

        var postLoginRedirectUrl = config["Authentication:Oidc:PostLoginRedirectUrl"]
            ?? throw new InvalidOperationException("Missing configuration: Authentication:Oidc:PostLoginRedirectUrl");

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "cfd.session";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Path = "/";
            options.LoginPath = "/api/auth/login";
            options.LogoutPath = "/api/auth/logout";
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
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

            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "https://schemas.campfitfurdogs.com/roles";

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProviderForSignOut = ctx =>
                {
                    // Standard OIDC logout (no Auth0-specific endpoint)
                    var logoutUri = $"{authority}/v2/logout?client_id={clientId}&returnTo={Uri.EscapeDataString(postLoginRedirectUrl)}";

                    ctx.Response.Redirect(logoutUri);
                    ctx.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void Configure(WebApplication app)
    {
        app.UseAuthentication();
    }
}
