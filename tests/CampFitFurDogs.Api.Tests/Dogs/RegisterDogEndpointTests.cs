using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

[Collection("API With Postgres")]
public class RegisterDogEndpointTests : ApiWithPostgresTestBase
{
    public RegisterDogEndpointTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    private sealed record DogResponse(Guid DogId);

    // ------------------------------------------------------------
    // SUCCESS — BASIC REGISTRATION
    // ------------------------------------------------------------
    [Fact]
    public async Task RegisterDog_ValidRequest_Returns201AndDogId()
    {
        var client = CreateClient();

        await AuthenticateAsync(client);

        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<DogResponse>();
        body.Should().NotBeNull();
        body!.DogId.Should().NotBeEmpty();
    }

    // ------------------------------------------------------------
    // VALIDATION — MISSING NAME
    // ------------------------------------------------------------
    [Fact]
    public async Task RegisterDog_MissingName_Returns400()
    {
        var client = CreateClient();

        await AuthenticateAsync(client);

        var request = new
        {
            Name = "",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ------------------------------------------------------------
    // AUTH — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task RegisterDog_MissingCustomerId_Returns400()
    {
        // No authentication → no session cookie
        var client = CreateClient();

        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
