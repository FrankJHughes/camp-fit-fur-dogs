using CampFitFurDogs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure.DbContexts;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureDbContexts(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services
            .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                })
            ;


    }
}
