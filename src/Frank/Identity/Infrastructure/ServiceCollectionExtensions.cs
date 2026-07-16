using Frank.Identity.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentity(this IServiceCollection services)
    {
        return services
            .AddScoped<IIdentityResolver, IdentityResolver>()
            .AddScoped<ICurrentUser, AuthenticatedUser>();
    }
}
