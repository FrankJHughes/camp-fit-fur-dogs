using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.EntityFrameworkCore.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityEntityFrameworkCoreSessions(this IServiceCollection services)
    {
        services
            .AddOptions<SessionSettings>()
            .BindConfiguration("Identity:Session")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddScoped<ICreateSessionWriter, CreateSessionWriter>()
            .AddScoped<IGetSessionReader, GetSessionReader>()
            .AddScoped<IRevokeSessionWriter, RevokeSessionWriter>();
    }
}
