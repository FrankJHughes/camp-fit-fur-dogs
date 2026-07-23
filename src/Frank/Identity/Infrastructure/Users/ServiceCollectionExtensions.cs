using Frank.Identity.Application.Abstractions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityInfrastructureUsers(this IServiceCollection services)
    {
        return services
            .AddScoped<ICurrentUser, CurrentUser>();
    }
}
