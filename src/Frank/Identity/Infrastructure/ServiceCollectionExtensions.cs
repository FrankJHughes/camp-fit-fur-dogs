using Frank.Identity.Infrastructure.Auth0;
using Frank.Identity.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityInfrastructure(this IServiceCollection services)
    {
        return services
            .AddFrankIdentityInfrastructureUsers()
            .AddFrankIdentityInfrastructureAuth0()
            ;
    }
}
