using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.DbContexts;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddDbContext<FrankIdentityDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                });
    }
}
