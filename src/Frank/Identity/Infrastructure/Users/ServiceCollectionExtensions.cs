using Frank.Identity.Application.Abstractions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure.Users;

/// <summary>
/// Provides extension methods for registering user‑related infrastructure
/// components used by the Identity subsystem.
/// <para>
/// This includes the <see cref="ICurrentUser"/> accessor, which exposes
/// information about the authenticated user for the current HTTP request.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the user infrastructure services for the Identity subsystem.
    /// <para>
    /// Adds:
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// <see cref="ICurrentUser"/> → <see cref="CurrentUser"/> (scoped)
    /// </description></item>
    /// </list>
    /// <para>
    /// The <c>Scoped</c> lifetime ensures that each HTTP request receives its own
    /// <see cref="CurrentUser"/> instance, aligned with the lifetime of
    /// <see cref="IHttpContextAccessor"/> and the underlying <see cref="HttpContext"/>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which user infrastructure services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityInfrastructureUsers(this IServiceCollection services)
    {
        return services
            .AddScoped<ICurrentUser, CurrentUser>();
    }
}
