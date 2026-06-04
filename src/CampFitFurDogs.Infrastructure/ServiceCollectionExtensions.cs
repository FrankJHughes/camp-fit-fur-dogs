using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Infrastructure.Audit;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Infrastructure.Identity.Oidc;
using CampFitFurDogs.Infrastructure.Time;
using Frank.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddFrankEfCore<AppDbContext>(
            [typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly]
        );

        // External identity resolver
        services.AddScoped<IIdentityResolver, OidcIdentityResolver>();

        services.AddHttpClient<IAuthClient, OidcAuthClient>();

        // ⭐ NEW: Audit logging (required for US‑110)
        services.AddSingleton<IAuditLogger, AuditLogger>();

        services.AddSingleton<ISystemClock, SystemClock>();


        return services;
    }
}
