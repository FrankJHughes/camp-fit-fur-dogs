using Microsoft.Extensions.DependencyInjection;
using Frank.Domain.Users;
using Frank.Application.Abstractions.Users.FindUserByExternalId;

namespace Frank.Infrastructure.EntityFrameworkCore.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankUsersInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IFindUserByExternalIdReader, FindUserByExternalIdReader>();
    }
}
