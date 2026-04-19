using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scrutor;

using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // 1. DbContext (explicit)
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        // 2. Repositories, services, adapters, interceptors (Scrutor)
        services.Scan(scan => scan
            .FromAssemblies(assembly)

            .AddClasses(c => c.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

            .AddClasses(c => c.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

            .AddClasses(c => c.Where(type => type.Name.EndsWith("Provider")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

            .AddClasses(c => c.Where(type => type.Name.EndsWith("Reader")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        // 3. Unit of Work (explicit — not matched by Scrutor suffixes)
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // 4. Current user service (explicit)
        services.AddSingleton<ICurrentUserService, DummyCurrentUserService>();

        return services;
    }
}

