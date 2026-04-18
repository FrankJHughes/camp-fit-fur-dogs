using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Api.Tests;

public class CampFitFurDogsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public TestCurrentUserService TestUserService { get; } = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's Npgsql registration
            // Remove ALL DbContextOptions<AppDbContext> registrations
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            // Point at the Testcontainers PostgreSQL instance
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));

            // Override ICurrentUserService with test double
            descriptors = services
                .Where(s => s.ServiceType == typeof(ICurrentUserService))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            services.AddSingleton<ICurrentUserService>(TestUserService);

            // Create schema from EF model
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
