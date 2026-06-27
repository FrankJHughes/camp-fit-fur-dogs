using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.Infrastructure.Sessions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSessionInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ISessionRepository, SessionRepository>();
    }
}
