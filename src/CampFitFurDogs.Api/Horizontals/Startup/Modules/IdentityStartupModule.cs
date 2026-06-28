using Frank.Abstractions.Startup;
using Frank.Infrastructure.Identity;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(30)]
public sealed class IdentityStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        _ = builder.Services
            .AddFrankIdentity();
    }

    public void Use(WebApplication app)
    {
    }
}
