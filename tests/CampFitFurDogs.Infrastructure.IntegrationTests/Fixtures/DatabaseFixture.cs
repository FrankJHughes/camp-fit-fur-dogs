using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Infrastructure.Data;
using Xunit;

namespace CampFitFurDogs.Infrastructure.IntegrationTests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    public AppDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not set.");

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var provider = services.BuildServiceProvider();

        DbContext = provider.GetRequiredService<AppDbContext>();

        await DbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
