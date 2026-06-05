using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Api.Tests.Fixtures;
using FluentAssertions;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class RemoveDogEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUser;

    public RemoveDogEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUser = Factory.TestUser;
    }

    private sealed record RegisterDogResponse(Guid dogId);

    private async Task<Guid> RegisterDogAsync()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

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
        return body!.dogId;
    }

    [Fact]
    public async Task RemoveDog_ShouldReturn204()
    {
        var dogId = await RegisterDogAsync();

        var response = await _client.DeleteAsync($"/api/dogs/{dogId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveDog_WhenDogNotFound_DoesNotReturn204()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var response = await _client.DeleteAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().NotBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveDog_ErrorResponse_DoesNotExposeInternals()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var response = await _client.DeleteAsync($"/api/dogs/{Guid.NewGuid()}");
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContain("stackTrace");
        body.Should().NotContain("innerException");
        body.Should().NotContain("ArgumentException");
        body.Should().NotContain("NullReferenceException");
    }
}
