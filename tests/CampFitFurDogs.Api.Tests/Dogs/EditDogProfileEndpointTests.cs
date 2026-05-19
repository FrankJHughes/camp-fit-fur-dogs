using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class EditDogProfileEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUser;

    public EditDogProfileEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUser = Factory.TestUser;
    }

    private sealed record RegisterDogResponse(Guid dogId);

    private async Task<Guid> RegisterDogAsync(Guid ownerId)
    {
        var request = new
        {
            name = "Biscuit",
            breed = "Golden Retriever",
            dateOfBirth = "2022-06-15",
            sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterDogResponse>();
        return body!.dogId;
    }

    private sealed record DogProfileResponse(
        Guid id,
        Guid ownerId,
        string name,
        string breed,
        DateOnly dateOfBirth,
        string sex);

    [Fact]
    public async Task EditDogProfile_ShouldReturn204()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerId);

        var request = new
        {
            name = "Waffles",
            breed = "Labrador",
            dateOfBirth = "2021-06-15",
            sex = "Male"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task EditDogProfile_WithEmptyName_Returns400()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerId);

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
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerId);

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
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerId);

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

    [Fact]
    public async Task EditDogProfile_ErrorResponse_DoesNotExposeInternals()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerId);

        var request = new
        {
            name = "",
            breed = "Beagle",
            dateOfBirth = "2022-08-20",
            sex = "Male"
        };

        var response = await _client.PutAsJsonAsync($"/api/dogs/{dogId}", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContain("stackTrace");
        body.Should().NotContain("innerException");
        body.Should().NotContain("ArgumentException");
        body.Should().NotContain("NullReferenceException");
    }
}
