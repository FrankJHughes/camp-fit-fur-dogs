using Frank.Core.Application.Abstractions.Startup;
using Frank.Core.Infrastructure.Identity;

namespace CampFitFurDogs.Api.StartupModules;

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
