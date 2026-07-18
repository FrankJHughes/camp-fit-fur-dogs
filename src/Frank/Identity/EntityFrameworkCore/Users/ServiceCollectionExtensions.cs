using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityUsersInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ICreateUserWriter, CreateUserWriter>()
            .AddScoped<IGetUserByExternalIdReader, GetUserByExternalIdReader>()
            .AddScoped<IGetUserByIdReader, GetUserByIdReader>();
    }
}
