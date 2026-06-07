using Testcontainers.PostgreSql;
using Xunit;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } =
        new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => Container.StartAsync();

    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}
