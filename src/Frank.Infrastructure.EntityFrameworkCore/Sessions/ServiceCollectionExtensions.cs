using Microsoft.Extensions.DependencyInjection;
using Frank.Domain.Sessions;

namespace Frank.Infrastructure.EntityFrameworkCore.Sessions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankSessionsInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ISessionRepository, SessionRepository>();
    }
}
