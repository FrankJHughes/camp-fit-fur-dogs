using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

[Collection("API With Postgres")]
public class EditDogProfileEndpointTests : ApiWithPostgresTestBase
{
    public EditDogProfileEndpointTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    // ------------------------------------------------------------
    // SUCCESS — OWNER UPDATES DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task EditDog_OwnerUpdatesDog_Returns204()
    {
        var client = CreateClient();

        // Authenticate using real AuthCallback pipeline
        await AuthenticateAsync(client);

        // Register dog as authenticated owner
        var dogId = await RegisterDogAsync(client, "Biscuit", "Golden Retriever");

        var request = new
        {
            Name = "Biscuit Updated",
            Breed = "Labrador",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ------------------------------------------------------------
    // AUTH — OTHER OWNER CANNOT EDIT
    // ------------------------------------------------------------
    [Fact]
    public async Task EditDog_OtherOwnerCannotEdit_Returns404()
    {
        // Owner A
        var clientA = Factory.CreateClientWithCookies();
        await AuthenticateAsync(clientA, "test|user-a-external-id");
        var dogId = await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B (different identity)
        var clientB = Factory.CreateClientWithCookies();
        await AuthenticateAsync(clientB, "test|user-b-external-id");

        var request = new
        {
            Name = "Hacked Name",
            Breed = "Hacked Breed",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await clientB.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ------------------------------------------------------------
    // VALIDATION — MISSING NAME
    // ------------------------------------------------------------
    [Fact]
    public async Task EditDog_MissingName_Returns400()
    {
        var client = CreateClient();
        await AuthenticateAsync(client);

        var dogId = await RegisterDogAsync(client, "Biscuit", "Golden Retriever");

        var request = new
        {
            Name = "",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
