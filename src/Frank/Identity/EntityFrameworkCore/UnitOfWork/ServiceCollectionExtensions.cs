using Frank.Identity.Application.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

/// <summary>
/// Provides extension methods for registering the Identity EntityFrameworkCore
/// unit of work implementation.
/// <para>
/// This extension adds <see cref="IFrankIdentityUnitOfWork"/> to the dependency
/// injection container, ensuring that Identity vertical slices can participate
/// in transactional boundaries coordinated by EF Core.
/// </para>
/// <para>
/// The unit of work is registered as <c>Scoped</c> because EF Core DbContexts
/// are scoped per request, and transactional consistency must align with that
/// lifetime.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the EF Core–backed Identity unit of work implementation.
    /// <para>
    /// Adds:
    /// <list type="bullet">
    /// <item><description><see cref="IFrankIdentityUnitOfWork"/> → <see cref="FrankIdentityUnitOfWork"/></description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to which the unit of work will be added.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// registration chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityEntityFrameworkCoreUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IFrankIdentityUnitOfWork, FrankIdentityUnitOfWork>();
    }
}
