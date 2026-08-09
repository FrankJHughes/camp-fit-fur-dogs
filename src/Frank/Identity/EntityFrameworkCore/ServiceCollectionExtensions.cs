using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Identity.EntityFrameworkCore.Sessions;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for registering all EntityFrameworkCore-backed
/// infrastructure components of the Identity subsystem.
/// <para>
/// This method acts as the root registration entry point, wiring together:
/// </para>
/// <list type="bullet">
/// <item><description>The Identity DbContext</description></item>
/// <item><description>The Identity Unit of Work</description></item>
/// <item><description>Session persistence (create, read, revoke)</description></item>
/// <item><description>User persistence (create, lookup by ID, lookup by external ID)</description></item>
/// </list>
/// <para>
/// By centralizing these registrations, the Identity subsystem can be added to
/// the application with a single, predictable call during startup.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all EntityFrameworkCore infrastructure services required by the
    /// Identity subsystem.
    /// <para>
    /// This includes:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="FrankIdentityDbContext"/></description></item>
    /// <item><description><see cref="IFrankIdentityUnitOfWork"/></description></item>
    /// <item><description>Session writers and readers</description></item>
    /// <item><description>User writers and readers</description></item>
    /// </list>
    /// <para>
    /// The method is intended to be called from application startup (e.g.,
    /// <c>Program.cs</c>) to ensure all Identity EF Core components are available
    /// for dependency injection.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to which Identity EF Core services will be added.</param>
    /// <param name="configuration">The application configuration used to bind DbContext settings.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// registration chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityEntityFrmeworkCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddFrankIdentityEntityFrameworkCoreDbContext(configuration)
            .AddFrankIdentityEntityFrameworkCoreUnitOfWork()
            .AddFrankIdentityEntityFrameworkCoreSessions()
            .AddFrankIdentityEntityFrameworkCoreUsers();
    }
}
