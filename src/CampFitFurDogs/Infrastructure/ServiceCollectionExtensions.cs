using CampFitFurDogs.Infrastructure.DbContexts;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure;

/// <summary>
/// Provides extension methods for registering all infrastructure‑layer services
/// used by the CampFitFurDogs application.
/// <para>
/// This method centralizes the wiring of:
/// <list type="bullet">
/// <item><description>Database contexts (<see cref="AddInfrastructureDbContexts"/>)</description></item>
/// <item><description>Dog persistence readers and writers (<see cref="AddInfrastructureDogs"/>)</description></item>
/// <item><description>Unit of Work (<see cref="AddInfrastructureUnitOfWork"/>)</description></item>
/// </list>
/// </para>
/// <para>
/// By grouping these registrations, the application startup remains clean and
/// each subsystem maintains clear separation of concerns.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all infrastructure‑layer components required by the
    /// CampFitFurDogs application.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Adds <c>IHttpContextAccessor</c> for request‑scoped operations.</description></item>
    /// <item><description>Registers <see cref="AppDbContext"/> using PostgreSQL.</description></item>
    /// <item><description>Registers all Dogs vertical‑slice readers and writers.</description></item>
    /// <item><description>Registers the EF Core Unit of Work implementation.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">The application configuration root.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all infrastructure
    /// services registered.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services
            .AddHttpContextAccessor()
            .AddInfrastructureDbContexts(configuration)
            .AddInfrastructureDogs()
            .AddInfrastructureUnitOfWork();
    }
}
