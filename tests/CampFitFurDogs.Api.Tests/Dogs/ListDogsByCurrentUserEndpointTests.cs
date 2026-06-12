using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

[Collection("API With Postgres")]
public class ListDogsByCurrentUserEndpointTests : ApiWithPostgresTestBase
{
    public ListDogsByCurrentUserEndpointTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    private sealed record DogSummaryDto(Guid Id, string Name, string Breed);
    private sealed record ListDogsResponseDto(List<DogSummaryDto> Dogs);

    // ------------------------------------------------------------
    // SUCCESS — MULTIPLE DOGS
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_OwnerHasMultipleDogs_Returns200WithAll()
    {
        var client = CreateClient();

        // Authenticate using real AuthCallback pipeline
        await AuthenticateAsync(client);

        // Register dogs as authenticated owner
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
        var client = CreateClient();

        await AuthenticateAsync(client);

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
        var clientA = CreateClient();
        await AuthenticateAsync(clientA);
        await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B
        var clientB = CreateClient();
        await AuthenticateAsync(clientB);
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
    // BAD REQUEST — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task ListDogs_MissingCustomerId_Returns400()
    {
        // No authentication → no session cookie
        var client = CreateClient();

        var response = await client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
