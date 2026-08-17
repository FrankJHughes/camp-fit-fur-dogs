using CampFitFurDogs.Application.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure.UnitOfWork;

/// <summary>
/// Provides extension methods for registering the infrastructure‑layer
/// unit‑of‑work implementation used by the CampFitFurDogs identity subsystem.
/// <para>
/// This class wires up <see cref="IAppUnitOfWork"/> with its EF Core‑backed
/// implementation <see cref="AppUnitOfWork"/>, ensuring that application‑layer
/// workflows have a consistent transactional boundary.
/// </para>
/// <para>
/// The registration follows the scoped lifetime convention, giving each
/// operation its own unit‑of‑work instance aligned with the underlying
/// <c>AppDbContext</c>.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the EF Core unit‑of‑work implementation with the dependency
    /// injection container.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Binds <see cref="IAppUnitOfWork"/> to <see cref="AppUnitOfWork"/>.</description></item>
    /// <item><description>Uses a scoped lifetime appropriate for EF Core operations.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with unit‑of‑work
    /// registration applied.
    /// </returns>
    public static IServiceCollection AddInfrastructureUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
    }
}
