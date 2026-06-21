using System.Net;
using System.Net.Http.Json;
using Frank.Testing.Contexts;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class RegisterDogEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;
    private HttpClient _client = default!;

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

        // 4. Create authenticated client
        var clientCtx = new ApiClientContext()
            .WithAuthenticatedUser($"Test0|{Guid.NewGuid()}");

        _client = _api.CreateClient(clientCtx);

        // 5. Verify identity is working (optional)
        var who = await _client.GetFromJsonAsync<WhoAmIResponse>("/__test__/current-user-id");
        who.Should().NotBeNull();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    // ------------------------------------------------------------
    // SUCCESS — BASIC REGISTRATION
    // ------------------------------------------------------------
    [Fact]
    public async Task RegisterDog_ValidRequest_Returns201AndDogId()
    {
        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);

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
        var request = new
        {
            Name = "",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ------------------------------------------------------------
    // AUTH — MISSING CUSTOMER ID
    // ------------------------------------------------------------
    [Fact]
    public async Task RegisterDog_MissingCustomerId_Returns401()
    {
        // Create anonymous client
        var anon = _api.CreateClient(new ApiClientContext());

        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await anon.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
