using Frank.Identity.Application.Callback;
using Frank.Identity.Application.Sessions;
using Frank.Identity.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplication(this IServiceCollection services)
    {
        return services

            .AddFrankIdentityApplicationSessions()

            .AddFrankIdentityApplicationUsers()

            .AddFrankIdentityApplicationCallback();

    }
}
