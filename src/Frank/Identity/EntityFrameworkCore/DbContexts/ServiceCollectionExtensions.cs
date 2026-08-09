using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.DbContexts;

/// <summary>
/// Provides extension methods for registering the Identity EF Core
/// <see cref="FrankIdentityDbContext"/> within a dependency injection container.
/// <para>
/// This extension centralizes the configuration required to connect the
/// Identity subsystem to its PostgreSQL database. It resolves the
/// <c>DefaultConnection</c> connection string from the application's
/// <see cref="IConfiguration"/> and applies it to the EF Core context.
/// </para>
/// <para>
/// The method is intended to be called from application startup code
/// (e.g., <c>Program.cs</c> or <c>Startup.cs</c>), ensuring that the Identity
/// DbContext is consistently configured across all hosting environments.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="FrankIdentityDbContext"/> with the dependency
    /// injection container using the PostgreSQL provider.
    /// <para>
    /// The connection string is retrieved from the application's configuration
    /// under the key <c>DefaultConnection</c>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the DbContext will be added.
    /// </param>
    /// <param name="configuration">
    /// The application configuration used to resolve the connection string.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// registration chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityEntityFrameworkCoreDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddDbContext<FrankIdentityDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
    }
}
