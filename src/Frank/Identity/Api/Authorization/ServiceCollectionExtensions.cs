#nullable enable

using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Authorization;

/// <summary>
/// Provides extension methods for configuring authorization services used by the
/// Identity API surface.
/// <para>
/// This subsystem establishes the default authorization behavior for identity‑related
/// endpoints, including a fallback policy that requires authenticated users for all
/// requests unless explicitly overridden.
/// Future policies (e.g., owner‑only access, verified‑email access) may be added as
/// additional identity stories are implemented.
/// </para>
/// </summary>
/// <remarks>
/// This configuration aligns with the Identity purity and safety rules described in
/// authentication and authorization stories such as US‑110, US‑111, US‑133, and US‑148:
/// <list type="bullet">
/// <item><description>All endpoints require authentication by default.</description></item>
/// <item><description>Authorization logic remains minimal and free of domain concerns.</description></item>
/// <item><description>Additional policies may be layered without modifying endpoint code.</description></item>
/// </list>
/// </remarks>
public static class AuthorizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers authorization services for the Identity API.
    /// <para>
    /// This method configures:
    /// <list type="bullet">
    /// <item><description>A fallback authorization policy requiring authenticated users.</description></item>
    /// <item><description>A builder for future identity‑specific authorization policies.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The fallback policy ensures that all Identity API endpoints are protected unless
    /// explicitly marked as anonymous.
    /// Additional policies (e.g., owner‑only access, verified‑email access) can be added
    /// as identity features evolve.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
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
