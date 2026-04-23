using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scrutor;

using SharedKernel.Abstractions;
using SharedKernel.Infrastructure.EntityFrameworkCore;

using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly;

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
}

