using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class GetDogProfileEndpointTests : IClassFixture<CampFitFurDogsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TestCurrentUserService _testUserService;

    public GetDogProfileEndpointTests(CampFitFurDogsApiFactory factory)
    {
        _client = factory.CreateClient();
        _testUserService = factory.TestUserService;
    }

    private sealed record DogProfileResponse(
        Guid Id, Guid OwnerId, string Name, string Breed,
        DateOnly DateOfBirth, string Sex);

    // ── AC-1: View all previously entered info ──

    [Fact]
    public async Task GetDogProfile_ExistingDogOwnedByCustomer_Returns200WithProfile()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUserService);
        var dogId = await RegisterDogAsync(_client, _testUserService, ownerId);

        var response = await _client.GetAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var profile = await response.Content.ReadFromJsonAsync<DogProfileResponse>();
        profile.Should().NotBeNull();
        profile!.Id.Should().Be(dogId);
        profile.OwnerId.Should().Be(ownerId);
        profile.Name.Should().Be("Biscuit");
        profile.Breed.Should().Be("Golden Retriever");
        profile.Sex.Should().Be("Female");
    }

    // ── AC-3: Missing dogs handled gracefully ──

    [Fact]
    public async Task GetDogProfile_NonExistentDog_Returns404()
    {
        var response = await _client.GetAsync(
            $"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── AC-2 + EG: Only owned dogs visible, no info leak ──

    [Fact]
    public async Task GetDogProfile_DogNotOwnedByCustomer_Returns404()
    {
        var ownerA = await CreateAndSetOwnerAsync(_client, _testUserService);
        var dogId = await RegisterDogAsync(_client, _testUserService, ownerA);
        var ownerB = await CreateAndSetOwnerAsync(_client, _testUserService);

        var response = await _client.GetAsync(
            $"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task GetDogProfile_MissingCustomerId_Returns400()
    {
        _testUserService.CurrentUserId = Guid.Empty;
        var response = await _client.GetAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
