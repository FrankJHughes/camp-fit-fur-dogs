using Frank.Core.Application.Abstractions.Startup;
using Frank.Identity.Infrastructure;
using Frank.Identity.Infrastructure.Auth0;

namespace CampFitFurDogs.Api.StartupModules;

[StartupModule(30)]
public sealed class IdentityStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        _ = builder.Services
            .AddFrankIdentityAuth0Infrastructure()
            .AddFrankIdentityInfrastructure();
    }

    public void Use(WebApplication app)
    {
    }
}
