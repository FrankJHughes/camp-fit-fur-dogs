using System.Net.Http;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Factories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public abstract class ApiWithPostgresTestBase
    : ApiWithoutDatabaseTestBase, IAsyncLifetime
{
    protected readonly PostgresFixture PostgresFixture;

    protected ApiWithPostgresTestBase(
        CampFitFurDogsApiFactory factory,
        PostgresFixture postgresFixture)
        : base(factory.Clone().UseContainer(postgresFixture.Container))
    {
        PostgresFixture = postgresFixture;
    }

    public async Task InitializeAsync()
    {
        using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tableNames = db.Model.GetEntityTypes()
            .Select(e => e.GetTableName())
            .Where(t => t != null)
            .Distinct()
            .Select(t => $"\"{t}\"")
            .ToList();

        if (tableNames.Count == 0)
            return;

        var joined = string.Join(", ", tableNames);

#pragma warning disable EF1002
        await db.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {joined} CASCADE;");
#pragma warning restore EF1002
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected HttpClient CreateClientWithOverrides(
        Action<IConfigurationBuilder>? overrides = null,
        WebApplicationFactoryClientOptions? options = null,
        Action<IServiceCollection>? configureServices = null)
    {
        // Start from a cloned, Postgres‑wired CampFitFurDogsApiFactory
        var factory = Factory.Clone().UseContainer(PostgresFixture.Container);

        // Apply config overrides on the concrete factory
        if (overrides is not null)
            factory.WithConfigOverrides(overrides);

        // If no service overrides, just create the client directly
        if (configureServices is null)
        {
            return factory.CreateClient(options ?? new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        // When overriding services, use the WebApplicationFactory<Program> returned
        WebApplicationFactory<Program> webFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(configureServices);
        });

        return webFactory.CreateClient(options ?? new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
