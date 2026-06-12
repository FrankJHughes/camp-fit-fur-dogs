using System.Net;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

[Collection("API With Postgres")]
public class RemoveDogEndpointTests : ApiWithPostgresTestBase
{
    public RemoveDogEndpointTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    // ------------------------------------------------------------
    // SUCCESS — OWNER REMOVES DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task RemoveDog_OwnerRemovesDog_Returns204()
    {
        var client = CreateClient();

        // Authenticate using real AuthCallback pipeline
        await AuthenticateAsync(client);

        // Register dog as authenticated owner
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
        var clientA = Factory.CreateClientWithCookies();
        await AuthenticateAsync(clientA, "test|user-a-external-id");
        var dogId = await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B (different identity)
        var clientB = Factory.CreateClientWithCookies();
        await AuthenticateAsync(clientB, "test|user-b-external-id");

        var response = await clientB.DeleteAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ------------------------------------------------------------
    // BAD REQUEST — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task RemoveDog_MissingCustomerId_Returns400()
    {
        // No authentication → no session cookie
        var client = CreateClient();

        var response = await client.DeleteAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
