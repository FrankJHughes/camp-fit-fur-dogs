using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Frank.TestUtilities.Fixtures;

public class PostgresFixture<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:17").Build();

    public TDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .EnableSensitiveDataLogging() // optional but useful for tests
            .Options;

        return (TDbContext)Activator.CreateInstance(typeof(TDbContext), options)!;
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Run migrations for this specific DbContext
        await using var ctx = CreateContext();
        await ctx.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
