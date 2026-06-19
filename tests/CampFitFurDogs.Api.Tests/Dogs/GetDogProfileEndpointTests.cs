using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;

using Frank.Testing.Contexts;

using static CampFitFurDogs.Api.Tests.Helpers.Dogs.DogHelper;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class GetDogProfileEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;

    private sealed record DogProfileDto(Guid Id, string Name, string Breed);

    // ------------------------------------------------------------
    // TEST INITIALIZATION
    // ------------------------------------------------------------
    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();

        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(true);

        _api = new ApiFactory(ctx);
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    // Helper: create authenticated client
    private HttpClient CreateAuthenticatedClient(string sub)
    {
        var clientCtx = new ApiClientContext()
            .WithAuthenticatedUser(sub);

        return _api.CreateClient(clientCtx);
    }

    // ------------------------------------------------------------
    // SUCCESS — OWNER GETS DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetDog_OwnerGetsDog_Returns200WithProfile()
    {
        var client = CreateAuthenticatedClient("test|owner-a");

        var dogId = await RegisterDogAsync(client, "Biscuit", "Golden Retriever");

        var response = await client.GetAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DogProfileDto>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(dogId);
        body.Name.Should().Be("Biscuit");
        body.Breed.Should().Be("Golden Retriever");
    }

    // ------------------------------------------------------------
    // AUTH — OTHER OWNER CANNOT SEE DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetDog_OtherOwnerCannotSeeDog_Returns404()
    {
        // Owner A
        var clientA = CreateAuthenticatedClient("test|owner-a");
        var dogId = await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B
        var clientB = CreateAuthenticatedClient("test|owner-b");

        var response = await clientB.GetAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ------------------------------------------------------------
    // AUTH — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetDog_MissingCustomerId_Returns401()
    {
        var anon = _api.CreateClient(new ApiClientContext());

        var response = await anon.GetAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
