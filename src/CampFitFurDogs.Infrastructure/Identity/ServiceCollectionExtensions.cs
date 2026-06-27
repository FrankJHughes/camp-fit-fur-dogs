using Microsoft.Extensions.DependencyInjection;
using Frank.Abstractions.Identity;

namespace CampFitFurDogs.Infrastructure.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IIdentityResolver, IdentityResolver>();
    }
}
