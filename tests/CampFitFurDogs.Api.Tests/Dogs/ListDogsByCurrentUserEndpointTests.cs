using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class ListDogsByCurrentUserEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUser;

    public ListDogsByCurrentUserEndpointTests(
        CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUser = Factory.TestUser;
    }

    private sealed record DogSummaryDto(Guid id, string name, string breed);
    private sealed record ListDogsResponseDto(List<DogSummaryDto> dogs);
    private sealed record RegisterDogResponse(Guid dogId);

    private async Task<Guid> RegisterDogAsync(Guid ownerId, string name, string breed)
    {
        var request = new
        {
            name = name,
            breed = breed,
            dateOfBirth = "2022-06-15",
            sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterDogResponse>();
        return body!.dogId;
    }

    [Fact]
    public async Task ListDogs_OwnerHasMultipleDogs_Returns200WithAll()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);

        var dog1Id = await RegisterDogAsync(ownerId, "Biscuit", "Golden Retriever");
        var dog2Id = await RegisterDogAsync(ownerId, "Maple", "Beagle");

        var response = await _client.GetAsync("/api/dogs");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.dogs.Should().HaveCount(2);

        body.dogs.Should().Contain(d =>
            d.id == dog1Id &&
            d.name == "Biscuit" &&
            d.breed == "Golden Retriever");

        body.dogs.Should().Contain(d =>
            d.id == dog2Id &&
            d.name == "Maple" &&
            d.breed == "Beagle");
    }

    [Fact]
    public async Task ListDogs_OwnerHasNoDogs_Returns200WithEmptyList()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUser);

        var response = await _client.GetAsync("/api/dogs");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.dogs.Should().BeEmpty();
    }

    [Fact]
    public async Task ListDogs_OnlyReturnsDogsForCurrentUser()
    {
        var ownerA = await CreateAndSetOwnerAsync(_client, _testUser);
        await RegisterDogAsync(ownerA, "Biscuit", "Golden Retriever");

        var ownerB = await CreateAndSetOwnerAsync(_client, _testUser);
        await RegisterDogAsync(ownerB, "Maple", "Beagle");

        _testUser.CurrentUserId = ownerA;

        var response = await _client.GetAsync("/api/dogs");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.dogs.Should().HaveCount(1);

        body.dogs[0].name.Should().Be("Biscuit");
    }

    [Fact]
    public async Task ListDogs_MissingCustomerId_Returns400()
    {
        _testUser.CurrentUserId = Guid.Empty;

        var response = await _client.GetAsync("/api/dogs");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
