using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure.IntegrationTests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    public FrankIdentityDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not set.");

        var services = new ServiceCollection();

        services.AddDbContext<FrankIdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        var provider = services.BuildServiceProvider();

        DbContext = provider.GetRequiredService<FrankIdentityDbContext>();

        await DbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
