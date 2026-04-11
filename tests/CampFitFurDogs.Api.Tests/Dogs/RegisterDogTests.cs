using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class RegisterDogTests : IClassFixture<CampFitFurDogsApiFactory>
{
    private readonly HttpClient _client;

    public RegisterDogTests(CampFitFurDogsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── Helper: create a customer so we have a valid OwnerId ──

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

    private sealed record OwnerResponse(Guid CustomerId);
    private sealed record RegisterDogResponse(Guid DogId);

    // ── AC: Successful registration ──

    [Fact]
    public async Task RegisterDog_ShouldReturn201AndDogId()
    {
        var ownerId = await CreateOwnerAsync();

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
        body.Should().NotBeNull();
        body!.DogId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task RegisterDog_ShouldHaveLocationHeader()
    {
        var ownerId = await CreateOwnerAsync();

        var request = new
        {
            OwnerId = ownerId,
            Name = "Maple",
            Breed = "Poodle",
            DateOfBirth = "2023-03-10",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith("/api/dogs/");
    }

    // ── Validation: 400 on bad input ──

    [Fact]
    public async Task RegisterDog_WithEmptyName_Returns400()
    {
        var ownerId = await CreateOwnerAsync();

        var request = new
        {
            OwnerId = ownerId,
            Name = "",
            Breed = "Labrador",
            DateOfBirth = "2021-01-01",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterDog_WithEmptyBreed_Returns400()
    {
        var ownerId = await CreateOwnerAsync();

        var request = new
        {
            OwnerId = ownerId,
            Name = "Biscuit",
            Breed = "",
            DateOfBirth = "2021-01-01",
            Sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterDog_WithInvalidSex_Returns400()
    {
        var ownerId = await CreateOwnerAsync();

        var request = new
        {
            OwnerId = ownerId,
            Name = "Biscuit",
            Breed = "Poodle",
            DateOfBirth = "2023-01-01",
            Sex = "Unknown"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── No internals exposed ──

    [Fact]
    public async Task RegisterDog_SuccessResponse_DoesNotExposeInternals()
    {
        var ownerId = await CreateOwnerAsync();

        var request = new
        {
            OwnerId = ownerId,
            Name = "Biscuit",
            Breed = "Beagle",
            DateOfBirth = "2022-08-20",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("aggregateVersion");
        body.Should().NotContainEquivalentOf("domainEvents");
        body.Should().NotContainEquivalentOf("EF Core");
    }

    [Fact]
    public async Task RegisterDog_ErrorResponse_DoesNotExposeInternals()
    {
        var ownerId = await CreateOwnerAsync();

        var request = new
        {
            OwnerId = ownerId,
            Name = "",
            Breed = "Beagle",
            DateOfBirth = "2022-08-20",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("stackTrace");
        body.Should().NotContainEquivalentOf("innerException");
        body.Should().NotContainEquivalentOf("ArgumentException");
        body.Should().NotContainEquivalentOf("NullReferenceException");
    }
}
