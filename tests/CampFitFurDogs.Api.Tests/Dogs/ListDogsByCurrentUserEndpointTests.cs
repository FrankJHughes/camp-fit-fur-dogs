using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Frank.Testing.Contexts;
using Testcontainers.PostgreSql;
using static CampFitFurDogs.Api.Tests.Helpers.Dogs.DogHelper;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class ListDogsByCurrentUserEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;
    private sealed record WhoAmIResponse(string UserId);
    private sealed record DogSummaryDto(Guid Id, string Name, string Breed);
    private sealed record ListDogsResponseDto(List<DogSummaryDto> Dogs);

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
        _api.StartServer();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
        {
            await _postgres.DisposeAsync();
        }
    }

    // Helper: create authenticated client
    private async Task<HttpClient> CreateAuthenticatedClient(string sub)
    {
        var clientCtx = new ApiClientContext()
            .WithAuthenticatedUser(sub);

        var client = _api.CreateClient(clientCtx);

        var who = await client.GetFromJsonAsync<WhoAmIResponse>("/__test__/current-user-id");
        who.Should().NotBeNull();

        return client;
    }

    // ------------------------------------------------------------
    // SUCCESS — MULTIPLE DOGS
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_OwnerHasMultipleDogs_Returns200WithAll()
    {
        var client = await CreateAuthenticatedClient("test|owner-a");

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
        var client = await CreateAuthenticatedClient("test|owner-a");

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
        var clientA = await CreateAuthenticatedClient("test|owner-a");
        await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B
        var clientB = await CreateAuthenticatedClient("test|owner-b");
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
    // AUTH — MISSING USER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_MissingUserId_Returns401()
    {
        var anon = _api.CreateClient(new ApiClientContext());

        var response = await anon.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
