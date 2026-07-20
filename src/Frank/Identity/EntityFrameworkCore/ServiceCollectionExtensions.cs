using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Identity.EntityFrameworkCore.Sessions;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityEntityFrmeworkCoreInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddFrankIdentityDbContext(configuration)
            .AddFrankIdentityUnitOfWorkInfrastructure()
            .AddFrankIdentityUsersInfrastructure()
            .AddFrankIdentitySessionsInfrastructure();
    }
}
