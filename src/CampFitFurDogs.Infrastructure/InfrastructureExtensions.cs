using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions.Audit;
using Frank.Abstractions.Identity;
using Frank.Infrastructure.EntityFrameworkCore;
using CampFitFurDogs.Infrastructure.Audit;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Infrastructure.Identity;

namespace CampFitFurDogs.Infrastructure;

public static class InfrastructureExtensions
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

        services.AddHttpContextAccessor();

        services.AddScoped<IIdentityResolver, IdentityResolver>();
        services.AddSingleton<IAuditLogger, AuditLogger>();

        return services;
    }
}
