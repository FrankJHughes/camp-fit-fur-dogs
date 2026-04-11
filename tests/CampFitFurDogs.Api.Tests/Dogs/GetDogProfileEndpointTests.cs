using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class GetDogProfileEndpointTests : IClassFixture<CampFitFurDogsApiFactory>
{
    private readonly HttpClient _client;

    public GetDogProfileEndpointTests(CampFitFurDogsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── Helpers ──

    private async Task<Guid> CreateOwnerAsync()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"owner-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<OwnerResponse>();
        return body!.CustomerId;
    }

    private async Task<Guid> RegisterDogAsync(Guid ownerId)
    {
        var request = new
        {
            OwnerId = ownerId,
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

    private sealed record OwnerResponse(Guid CustomerId);
    private sealed record RegisterDogResponse(Guid DogId);
    private sealed record DogProfileResponse(
        Guid Id, Guid OwnerId, string Name, string Breed,
        DateOnly DateOfBirth, string Sex);

    // ── AC-1: View all previously entered info ──

    [Fact]
    public async Task GetDogProfile_ExistingDogOwnedByCustomer_Returns200WithProfile()
    {
        var ownerId = await CreateOwnerAsync();
        var dogId = await RegisterDogAsync(ownerId);

        var response = await _client.GetAsync($"/api/dogs/{dogId}?customerId={ownerId}");

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
            $"/api/dogs/{Guid.NewGuid()}?customerId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── AC-2 + EG: Only owned dogs visible, no info leak ──

    [Fact]
    public async Task GetDogProfile_DogNotOwnedByCustomer_Returns404()
    {
        var ownerA = await CreateOwnerAsync();
        var dogId = await RegisterDogAsync(ownerA);
        var ownerB = await CreateOwnerAsync();

        var response = await _client.GetAsync(
            $"/api/dogs/{dogId}?customerId={ownerB}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Validation ──

    [Fact]
    public async Task GetDogProfile_MissingCustomerId_Returns400()
    {
        var response = await _client.GetAsync($"/api/dogs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
