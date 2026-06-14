using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_DomainInvariantTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();

        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false);

        _api = new ApiFactory(ctx);
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    private HttpClient CreateClient()
        => _api.CreateClient(new ApiClientContext());

    // ------------------------------------------------------------
    // Add domain invariant tests here...
    // ------------------------------------------------------------

    // Example placeholder (remove when adding real tests)
    [Fact(Skip = "Add real domain invariant tests for CreateCustomer")]
    public void Placeholder()
    {
        true.Should().BeTrue();
    }
}
