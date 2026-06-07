using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Fakes;
using CampFitFurDogs.TestUtilities.Infrastructure;
using Frank.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.TestUtilities.Factories;

public class CampFitFurDogsApiFactory : WebApplicationFactory<Program>
{
    private readonly List<Action<IConfigurationBuilder>> _configOverrides = new();
    private string? _environmentOverride;

    public PostgreSqlContainer? Postgres { get; private set; }

    public TestCurrentUser TestUser { get; } = new();

    public void UseContainer(PostgreSqlContainer container)
        => Postgres = container;

    internal void InternalConfigOverrides(Action<IConfigurationBuilder> configure)
        => _configOverrides.Add(configure);

    internal void InternalSetEnvironment(string environment)
        => _environmentOverride = environment;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        //
        // ⭐ Program.cs reads Application Configuration.
        //    All overrides MUST be applied here.
        //
        builder.ConfigureAppConfiguration(cfg =>
        {
            //
            // ⭐ Default values required for the app to boot
            //
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Required by CorsStartupModule
                ["Frontend__BaseUrl"] = "http://localhost:3000",

                // Required by AuthenticationStartupModule
                ["Authentication:Oidc:Authority"] = "https://test.example.com",
                ["Authentication:Oidc:ClientId"] = "test-client-id",
                ["Authentication:Oidc:ClientSecret"] = "test-secret",
                ["Authentication:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
                ["Authentication:Oidc:PostLoginRedirectUrl"] = "http://localhost:3000/"
            });

            //
            // ⭐ Apply test overrides LAST so they always win
            //
            foreach (var apply in _configOverrides)
                apply(cfg);
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(WebHostDefaults.ApplicationKey, Guid.NewGuid().ToString());

        //
        // ⭐ Environment override (default to Development)
        //
        builder.UseEnvironment(_environmentOverride ?? "Development");

        builder.ConfigureServices(services =>
        {
            if (Postgres is null)
                throw new InvalidOperationException("Postgres container not initialized.");

            //
            // ⭐ Replace DbContext with Testcontainers PostgreSQL
            //
            var dbDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();

            foreach (var d in dbDescriptors)
                services.Remove(d);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Postgres.GetConnectionString()));

            //
            // ⭐ Replace ICurrentUserService with test double
            //
            var userDescriptors = services
                .Where(d => d.ServiceType == typeof(ICurrentUserService))
                .ToList();

            foreach (var descriptor in userDescriptors)
                services.Remove(descriptor);

            services.AddScoped<ICurrentUserService>(_ => TestUser);

            //
            // ⭐ Ensure schema is created
            //
            services.AddHostedService<TestDatabaseInitializer>();
        });
    }
}
