using CampFitFurDogs.Infrastructure.Sessions;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Core.Application.Settings;
using Frank.Identity.Domain.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

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
