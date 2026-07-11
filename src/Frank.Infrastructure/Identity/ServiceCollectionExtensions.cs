using Frank.Abstractions.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentity(this IServiceCollection services)
    {
        return services
            .AddScoped<IIdentityResolver, IdentityResolver>()
            .AddScoped<ICurrentUser, AuthenticatedUser>();
    }
}
