using Frank.Abstractions.Startup;

namespace CampFitFurDogs.Api.Horizontal.Startup.Modules;

[StartupModule(80)]
public class AuthorizationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var authorizationBuilder = services.AddAuthorizationBuilder();

        // Fallback policy: require authenticated user for all endpoints
        authorizationBuilder.AddFallbackPolicy("Require Authenticated User Policy", policy =>
            policy.RequireAuthenticatedUser());

        // Future policies (US‑133, US‑148, etc.) go here:
        //
        // auth.AddPolicy("OwnerOnly", policy =>
        //     policy.RequireClaim(ClaimTypes.NameIdentifier));
    }

    public void Use(WebApplication app)
    {
        app.UseAuthorization();
    }
}
