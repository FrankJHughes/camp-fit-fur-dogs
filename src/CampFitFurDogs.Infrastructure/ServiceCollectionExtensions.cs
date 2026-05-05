using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SharedKernel.Infrastructure.EntityFrameworkCore;

using CampFitFurDogs.Infrastructure.Data;


namespace CampFitFurDogs.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        var assembly = typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly;

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddSharedKernelEfCore<AppDbContext>();

        return services;
    }
}

