using Frank.Application.Abstractions.Users.FindUserByExternalId;
using Frank.Application.Abstractions.Users.GetUserById;
using Frank.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore.Users;

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
