using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;
using static CampFitFurDogs.Api.Tests.Helpers.Dogs.DogHelper;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class ListDogsByCurrentUserEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;

    private sealed record DogSummaryDto(Guid Id, string Name, string Breed);
    private sealed record ListDogsResponseDto(List<DogSummaryDto> Dogs);

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
    // SUCCESS — MULTIPLE DOGS
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_OwnerHasMultipleDogs_Returns200WithAll()
    {
        var client = CreateAuthenticatedClient("test|owner-a");

        var dog1Id = await RegisterDogAsync(client, "Biscuit", "Golden Retriever");
        var dog2Id = await RegisterDogAsync(client, "Maple", "Beagle");

        var response = await client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.Dogs.Should().HaveCount(2);

        body.Dogs.Should().Contain(d =>
            d.Id == dog1Id &&
            d.Name == "Biscuit" &&
            d.Breed == "Golden Retriever");

        body.Dogs.Should().Contain(d =>
            d.Id == dog2Id &&
            d.Name == "Maple" &&
            d.Breed == "Beagle");
    }

    // ------------------------------------------------------------
    // SUCCESS — EMPTY LIST
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_OwnerHasNoDogs_Returns200WithEmptyList()
    {
        var client = CreateAuthenticatedClient("test|owner-a");

        var response = await client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.Dogs.Should().BeEmpty();
    }

    // ------------------------------------------------------------
    // FILTERING — ONLY CURRENT USER'S DOGS
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_OnlyReturnsDogsForCurrentUser()
    {
        // Owner A
        var clientA = CreateAuthenticatedClient("test|owner-a");
        await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B
        var clientB = CreateAuthenticatedClient("test|owner-b");
        await RegisterDogAsync(clientB, "Maple", "Beagle");

        // Query as Owner A
        var response = await clientA.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.Dogs.Should().HaveCount(1);

        body.Dogs[0].Name.Should().Be("Biscuit");
    }

    // ------------------------------------------------------------
    // AUTH — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_MissingCustomerId_Returns401()
    {
        var anon = _api.CreateClient(new ApiClientContext());

        var response = await anon.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
