using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Application.Abstractions.Users;
using Frank.Identity.Application.Sessions;
using Frank.Identity.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<ISessionTokenGenerator, SessionTokenGenerator>()
            .AddScoped<IUserResolver, UserResolver>()
            .AddFrankCqrsCommands([
                typeof(Frank.Identity.Application.AssemblyMarker).Assembly
            ])
            .AddFrankCqrsQueries([
                typeof(Frank.Identity.Application.AssemblyMarker).Assembly
            ]);
    }
}
