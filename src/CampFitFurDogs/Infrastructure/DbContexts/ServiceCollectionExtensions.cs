using CampFitFurDogs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure.DbContexts;

/// <summary>
/// Provides extension methods for registering infrastructure‑layer database
/// contexts used by the CampFitFurDogs application.
/// <para>
/// This class wires up the <see cref="AppDbContext"/> using the connection
/// string supplied by the application's configuration system.
/// </para>
/// <para>
/// The registration uses the scoped lifetime convention, ensuring each request
/// or operation receives its own <see cref="AppDbContext"/> instance.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="AppDbContext"/> with the dependency injection
    /// container using the Npgsql provider.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Retrieves the <c>DefaultConnection</c> connection string.</description></item>
    /// <item><description>Configures EF Core to use PostgreSQL via <c>UseNpgsql</c>.</description></item>
    /// <item><description>Adds <see cref="AppDbContext"/> with a scoped lifetime.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">The application's configuration root.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with database context
    /// registrations applied.
    /// </returns>
    public static IServiceCollection AddInfrastructureDbContexts(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
    }
}
