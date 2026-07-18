using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Application.Settings;
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
            .AddScoped<ICreateSessionWriter, CreateSessionWriter>()
            .AddScoped<IGetSessionReader, GetSessionReader>()
            .AddScoped<IRevokeSessionWriter, RevokeSessionWriter>();
    }
}
