using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using Testcontainers.PostgreSql;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Api.Tests.Fixtures;

public class CampFitFurDogsApiFactory : WebApplicationFactory<Program>
{
    public PostgreSqlContainer? Postgres { get; private set; }

    public void UseContainer(PostgreSqlContainer container)
        => Postgres = container;

    // Test double (IMPORTANT: do NOT end with "Service")
    public TestCurrentUser TestUser { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //
        // Force a fresh DI container per test class
        //
        builder.UseSetting(WebHostDefaults.ApplicationKey, Guid.NewGuid().ToString());

        builder.ConfigureServices(services =>
        {
            if (Postgres is null)
                throw new InvalidOperationException("Postgres container not initialized.");

            //
            // 1. Remove existing DbContext registrations
            //
            var dbDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();

            foreach (var d in dbDescriptors)
                services.Remove(d);

            //
            // 2. Register DbContext pointing at Testcontainers PostgreSQL
            //
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Postgres.GetConnectionString()));

            //
            // 3. Replace ICurrentUserService with test double
            //
            var descriptors =
                services
                .Where(
                    d => d.ServiceType == typeof(ICurrentUserService))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                if (descriptor != null)
                    services.Remove(descriptor);
            }

            // Correct: instance override using lambda to force the instance overload
            services.AddScoped<ICurrentUserService>(_ => TestUser);

            //
            // 4. Register a hosted service that creates the schema
            //
            services.AddHostedService<TestDatabaseInitializer>();
        });
    }
}
