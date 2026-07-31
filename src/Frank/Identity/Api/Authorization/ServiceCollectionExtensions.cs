using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Authorization;

public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApiAuthorization(this IServiceCollection services)
    {
        var authorizationBuilder = services.AddAuthorizationBuilder();

        // Fallback policy: require authenticated user for all endpoints
        authorizationBuilder.AddFallbackPolicy("RequireAuthenticatedUser", policy =>
            policy.RequireAuthenticatedUser());

        // Future policies (US‑133, US‑148, etc.)
        // authorizationBuilder.AddPolicy("OwnerOnly", policy =>
        //     policy.RequireClaim(ClaimTypes.NameIdentifier));

        return services;
    }
}
