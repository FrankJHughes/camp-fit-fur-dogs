using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CampFitFurDogs.TestUtilities.Infrastructure;

public sealed class TestDatabaseInitializer<TContext> : IHostedService
    where TContext : DbContext
{
    private readonly IServiceProvider _provider;

    public TestDatabaseInitializer(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();

        await db.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
