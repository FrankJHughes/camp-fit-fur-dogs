using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SharedKernel.Infrastructure.EntityFrameworkCore;

using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Infrastructure.Audit;
using CampFitFurDogs.Infrastructure.Identity.Oidc;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Infrastructure.Time;

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

        services.AddSharedKernelEfCore<AppDbContext>(
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
