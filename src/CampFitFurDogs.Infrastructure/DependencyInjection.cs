using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Infrastructure.Dogs;

namespace CampFitFurDogs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDogRepository, DogRepository>();

        return services;
    }
}
