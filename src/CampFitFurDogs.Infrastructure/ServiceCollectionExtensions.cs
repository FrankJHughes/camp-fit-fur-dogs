using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SharedKernel.Infrastructure.EntityFrameworkCore;

using CampFitFurDogs.Infrastructure.Data;
using SharedKernel.DependencyInjection;


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

        return services;
    }
}

