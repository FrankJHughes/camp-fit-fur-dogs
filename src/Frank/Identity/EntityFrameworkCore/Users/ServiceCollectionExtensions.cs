using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Users;

/// <summary>
/// Provides extension methods for registering all Entity Framework Core–based
/// user persistence components within the Identity subsystem.
/// <para>
/// This extension registers the writers and readers that make up the Users
/// vertical slice, enabling user creation and lookup operations backed by
/// <see cref="FrankIdentityDbContext"/>.
/// </para>
/// <para>
/// All services are registered with a <c>Scoped</c> lifetime to align with the
/// lifetime of the underlying DbContext and ensure transactional consistency.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core user services for the Identity subsystem.
    /// <para>
    /// The following services are added:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="ICreateUserWriter"/> → <see cref="CreateUserWriter"/></description></item>
    /// <item><description><see cref="IGetUserByExternalIdReader"/> → <see cref="GetUserByExternalIdReader"/></description></item>
    /// <item><description><see cref="IGetUserByIdReader"/> → <see cref="GetUserByIdReader"/></description></item>
    /// </list>
    /// <para>
    /// These components form the infrastructure layer for user creation and lookup
    /// vertical slices, ensuring that domain aggregates are persisted and retrieved
    /// correctly through EF Core.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to which the user services will be added.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// registration chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityEntityFrameworkCoreUsers(this IServiceCollection services)
    {
        return services
            .AddScoped<ICreateUserWriter, CreateUserWriter>()
            .AddScoped<IGetUserByExternalIdReader, GetUserByExternalIdReader>()
            .AddScoped<IGetUserByIdReader, GetUserByIdReader>();
    }
}
