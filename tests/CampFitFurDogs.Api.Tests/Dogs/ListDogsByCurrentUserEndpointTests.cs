using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using CampFitFurDogs.Api.Tests.Fixtures;

using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class ListDogsByCurrentUserEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUserService;

    public ListDogsByCurrentUserEndpointTests(
        CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUserService = Factory.TestUser;
    }

    private sealed record DogSummaryDto(Guid Id, string Name, string Breed);
    private sealed record ListDogsResponseDto(List<DogSummaryDto> Dogs);

    [Fact]
    public async Task ListDogs_OwnerHasMultipleDogs_Returns200WithAll()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUserService);
        var dog1Id = await RegisterDogAsync(
            _client, _testUserService, ownerId,
            "Biscuit", "Golden Retriever");
        var dog2Id = await RegisterDogAsync(
            _client, _testUserService, ownerId,
            "Maple", "Beagle");

        var response = await _client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.Dogs.Should().HaveCount(2);
        body.Dogs.Should().Contain(d =>
            d.Id == dog1Id && d.Name == "Biscuit" && d.Breed == "Golden Retriever");
        body.Dogs.Should().Contain(d =>
            d.Id == dog2Id && d.Name == "Maple" && d.Breed == "Beagle");
    }

    [Fact]
    public async Task ListDogs_OwnerHasNoDogs_Returns200WithEmptyList()
    {
        var ownerId = await CreateAndSetOwnerAsync(_client, _testUserService);

        var response = await _client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.Dogs.Should().BeEmpty();
    }

    [Fact]
    public async Task ListDogs_OnlyReturnsDogsForCurrentUser()
    {
        var ownerA = await CreateAndSetOwnerAsync(_client, _testUserService);
        await RegisterDogAsync(
            _client, _testUserService, ownerA,
            "Biscuit", "Golden Retriever");

        var ownerB = await CreateAndSetOwnerAsync(_client, _testUserService);
        await RegisterDogAsync(
            _client, _testUserService, ownerB,
            "Maple", "Beagle");

        // Switch back to ownerA
        _testUserService.CurrentUserId = ownerA;

        var response = await _client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<ListDogsResponseDto>();
        body.Should().NotBeNull();
        body!.Dogs.Should().HaveCount(1);
        body.Dogs[0].Name.Should().Be("Biscuit");
    }

    [Fact]
    public async Task ListDogs_MissingCustomerId_Returns400()
    {
        _testUserService.CurrentUserId = Guid.Empty;

        var response = await _client.GetAsync("/api/dogs");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
