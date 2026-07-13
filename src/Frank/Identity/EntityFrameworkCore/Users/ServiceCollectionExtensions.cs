using Frank.Identity.Application.Abstractions.Users.FindUserByExternalId;
using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Frank.Identity.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankUsersInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IFindUserByExternalIdReader, FindUserByExternalIdReader>()
            .AddScoped<IGetUserByIdReader, GetUserByIdReader>();
    }
}
