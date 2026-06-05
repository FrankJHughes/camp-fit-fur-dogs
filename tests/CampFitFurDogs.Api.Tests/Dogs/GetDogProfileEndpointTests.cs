using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Api.Tests.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class GetDogProfileEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUser;

    public GetDogProfileEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUser = Factory.TestUser;
    }

    private sealed record DogProfileResponse(
        Guid id,
        Guid ownerId,
        string name,
        string breed,
        DateOnly dateOfBirth,
        string sex);

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

    [Fact]
    public async Task GetDogProfile_ExistingDogOwnedByCustomer_Returns200WithProfile()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerId);

        var response = await _client.GetAsync($"/api/dogs/{dogId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var profile = await response.Content.ReadFromJsonAsync<DogProfileResponse>();
        profile.Should().NotBeNull();

        profile!.id.Should().Be(dogId);
        profile.ownerId.Should().Be(ownerId);
        profile.name.Should().Be("Biscuit");
        profile.breed.Should().Be("Golden Retriever");
        profile.sex.Should().Be("Female");
        profile.dateOfBirth.Should().Be(new DateOnly(2022, 6, 15));
    }

    [Fact]
    public async Task GetDogProfile_NonExistentDog_Returns404()
    {
        var response = await _client.GetAsync($"/api/dogs/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDogProfile_DogNotOwnedByCustomer_Returns404()
    {
        var ownerA = await CreateAndSetOwnerAsync(_client, _testUser);
        var dogId = await RegisterDogAsync(ownerA);

        var ownerB = await CreateAndSetOwnerAsync(_client, _testUser);

        var response = await _client.GetAsync($"/api/dogs/{dogId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDogProfile_MissingCustomerId_Returns400()
    {
        _testUser.CurrentUserId = Guid.Empty;

        var response = await _client.GetAsync($"/api/dogs/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
