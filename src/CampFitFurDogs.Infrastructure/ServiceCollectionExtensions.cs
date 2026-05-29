using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SharedKernel.Infrastructure.EntityFrameworkCore;

using CampFitFurDogs.Infrastructure.Data;
using SharedKernel.DependencyInjection;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication.Auth0;
using CampFitFurDogs.Application.Abstractions.Identity.External;
using CampFitFurDogs.Infrastructure.Audit;
using CampFitFurDogs.Infrastructure.Identity.Auth0;

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
        services.AddScoped<IExternalIdentityResolver, Auth0IdentityResolver>();

        services.AddHttpClient<IAuth0Client, Auth0Client>();

        // ⭐ NEW: Audit logging (required for US‑110)
        services.AddSingleton<IAuditLogger, AuditLogger>();

        return services;
    }
}
