using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class EditDogProfileEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUserService;

    public EditDogProfileEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUserService = Factory.TestUser;
    }

    private async Task<Guid> RegisterDogAsync()
    {
        await CreateAndSetOwnerAsync(_client, _testUserService);

        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterDogResponse>();
        return body!.DogId;
    }

    // ── AC: Successful edit ──

    [Fact]
    public async Task EditDogProfile_ShouldReturn204()
    {
        var dogId = await RegisterDogAsync();

        var request = new
        {
            Name = "Waffles",
            Breed = "Labrador",
            DateOfBirth = "2021-06-15",
            Sex = "Male"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ── Validation: 400 on bad input ──

    [Fact]
    public async Task EditDogProfile_WithEmptyName_Returns400()
    {
        var dogId = await RegisterDogAsync();

        var request = new
        {
            Name = "",
            Breed = "Labrador",
            DateOfBirth = "2021-01-01",
            Sex = "Male"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditDogProfile_WithEmptyBreed_Returns400()
    {
        var dogId = await RegisterDogAsync();

        var request = new
        {
            Name = "Waffles",
            Breed = "",
            DateOfBirth = "2021-01-01",
            Sex = "Female"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditDogProfile_WithInvalidSex_Returns400()
    {
        var dogId = await RegisterDogAsync();

        var request = new
        {
            Name = "Waffles",
            Breed = "Poodle",
            DateOfBirth = "2023-01-01",
            Sex = "Unknown"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── No internals exposed ──

    [Fact]
    public async Task EditDogProfile_ErrorResponse_DoesNotExposeInternals()
    {
        var dogId = await RegisterDogAsync();

        var request = new
        {
            Name = "",
            Breed = "Beagle",
            DateOfBirth = "2022-08-20",
            Sex = "Male"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("stackTrace");
        body.Should().NotContainEquivalentOf("innerException");
        body.Should().NotContainEquivalentOf("ArgumentException");
        body.Should().NotContainEquivalentOf("NullReferenceException");
    }
}
