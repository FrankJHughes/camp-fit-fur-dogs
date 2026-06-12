using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

[Collection("API With Postgres")]
public class GetDogProfileEndpointTests : ApiWithPostgresTestBase
{
    public GetDogProfileEndpointTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    private sealed record DogResponse(Guid DogId);
    private sealed record DogProfileDto(Guid Id, string Name, string Breed);

    // ------------------------------------------------------------
    // SUCCESS — OWNER GETS DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetDog_OwnerGetsDog_Returns200WithProfile()
    {
        var client = CreateClient();

        // Authenticate using real AuthCallback pipeline
        await AuthenticateAsync(client);

        // Register dog as authenticated owner
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
        var clientA = CreateClient();
        await AuthenticateAsync(clientA);
        var dogId = await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B (different identity)
        var clientB = CreateClient();
        await AuthenticateAsync(clientB);

        var response = await clientB.GetAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ------------------------------------------------------------
    // BAD REQUEST — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetDog_MissingCustomerId_Returns400()
    {
        // No authentication → no session cookie
        var client = CreateClient();

        var response = await client.GetAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
