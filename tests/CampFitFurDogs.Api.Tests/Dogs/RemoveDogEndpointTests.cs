using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;
using static CampFitFurDogs.Api.Tests.Helpers.Dogs.DogHelper;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class RemoveDogEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;

    private sealed record DogResponse(Guid DogId);
    private sealed record WhoAmIResponse(string UserId);

    // ------------------------------------------------------------
    // TEST INITIALIZATION
    // ------------------------------------------------------------
    public async Task InitializeAsync()
    {
        // 1. Start Postgres
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();

        // 2. Build ApiContext
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(true);

        // 3. Create ApiFactory
        _api = new ApiFactory(ctx);
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    // Helper: create an authenticated client
    private HttpClient CreateAuthenticatedClient(string sub)
    {
        var clientCtx = new ApiClientContext()
            .WithAuthenticatedUser(sub);

        return _api.CreateClient(clientCtx);
    }

    // ------------------------------------------------------------
    // SUCCESS — OWNER REMOVES DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task RemoveDog_OwnerRemovesDog_Returns204()
    {
        var client = CreateAuthenticatedClient("test|owner-a");

        var dogId = await RegisterDogAsync(client, "Biscuit", "Golden Retriever");

        var response = await client.DeleteAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ------------------------------------------------------------
    // AUTH — OTHER OWNER CANNOT REMOVE
    // ------------------------------------------------------------
    [Fact]
    public async Task RemoveDog_OtherOwnerCannotRemove_Returns404()
    {
        // Owner A
        var clientA = CreateAuthenticatedClient("test|owner-a");
        var dogId = await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B
        var clientB = CreateAuthenticatedClient("test|owner-b");

        var response = await clientB.DeleteAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ------------------------------------------------------------
    // AUTH — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task RemoveDog_MissingCustomerId_Returns401()
    {
        var anon = _api.CreateClient(new ApiClientContext());

        var response = await anon.DeleteAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
