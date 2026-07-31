using Frank.Core.Api.Middleware;
using Frank.Identity.Api.Authentication;
using Frank.Identity.Api.Authorization;
using Frank.Identity.Application;
using Frank.Identity.EntityFrameworkCore;
using Frank.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Platform;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddFrankIdentityApiAuthentication(configuration)
            .AddFrankIdentityApiAuthorization()
            .AddFrankIdentityApplication()
            .AddFrankIdentityEntityFrmeworkCore(configuration)
            .AddFrankIdentityInfrastructure()
            .AddFrankCoreApiMiddleware()
            ;

    }
}
