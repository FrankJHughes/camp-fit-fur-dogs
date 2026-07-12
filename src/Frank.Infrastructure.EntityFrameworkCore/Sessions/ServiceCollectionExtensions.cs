using CampFitFurDogs.Infrastructure.Sessions;
using Frank.Application.Abstractions.Sessions.GetSession;
using Frank.Application.Settings;
using Frank.Domain.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore.Sessions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankSessionsInfrastructure(this IServiceCollection services)
    {
        services
            .AddOptions<SessionSettings>()
            .BindConfiguration("Authentication:Session")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddScoped<ISessionRepository, SessionRepository>()
            .AddScoped<IGetSessionReader, GetSessionReader>();
    }
}
