using System.Net;
using System.Net.Http.Json;
using Frank.Testing.Contexts;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;
using static CampFitFurDogs.Api.Tests.Helpers.Dogs.DogHelper;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class EditDogProfileEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;

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

    private sealed record DogProfileDto(Guid Id, string Name, string Breed);

    // ------------------------------------------------------------
    // SUCCESS — OWNER UPDATES DOG
    // ------------------------------------------------------------
    [Fact]
    public async Task EditDog_OwnerUpdatesDog_Returns204()
    {
        var client = CreateAuthenticatedClient("test|owner-a");

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
        var clientA = CreateAuthenticatedClient("test|owner-a");
        var dogId = await RegisterDogAsync(clientA, "Biscuit", "Golden Retriever");

        // Owner B
        var clientB = CreateAuthenticatedClient("test|owner-b");

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
        var client = CreateAuthenticatedClient("test|owner-a");

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
