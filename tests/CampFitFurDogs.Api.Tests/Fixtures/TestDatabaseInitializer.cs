using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Api.Tests.Fixtures;

public class TestDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _provider;

    public TestDatabaseInitializer(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
