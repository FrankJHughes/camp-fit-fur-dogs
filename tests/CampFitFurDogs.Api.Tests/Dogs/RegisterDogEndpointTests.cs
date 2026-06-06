using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using static CampFitFurDogs.Api.Tests.ApiTestHelpers;

namespace CampFitFurDogs.Api.Tests.Dogs;

public class RegisterDogEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;
    private readonly TestCurrentUser _testUser;

    public RegisterDogEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
        _testUser = Factory.TestUser;
    }

    private sealed record RegisterDogResponse(Guid dogId);

    [Fact]
    public async Task RegisterDog_ShouldReturn201AndDogId()
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
        body.Should().NotBeNull();
        body!.dogId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task RegisterDog_ShouldHaveLocationHeader()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var request = new
        {
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

    [Fact]
    public async Task RegisterDog_WithEmptyName_Returns400()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var request = new
        {
            Name = "",
            Breed = "Labrador",
            DateOfBirth = "2021-01-01",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task RegisterDog_WithEmptyBreed_Returns400()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var request = new
        {
            Name = "Biscuit",
            Breed = "",
            DateOfBirth = "2021-01-01",
            Sex = "Female"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Breed");
    }

    [Fact]
    public async Task RegisterDog_WithInvalidSex_Returns400()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Unknown"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Sex");
    }

    [Fact]
    public async Task RegisterDog_SuccessResponse_DoesNotExposeInternals()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var request = new
        {
            Name = "Biscuit",
            Breed = "Beagle",
            DateOfBirth = "2022-08-20",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContain("aggregateVersion");
        body.Should().NotContain("domainEvents");
        body.Should().NotContain("EF Core");
    }

    [Fact]
    public async Task RegisterDog_ErrorResponse_DoesNotExposeInternals()
    {
        await CreateAndSetOwnerAsync(_client, _testUser);

        var request = new
        {
            Name = "",
            Breed = "Beagle",
            DateOfBirth = "2022-08-20",
            Sex = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/dogs", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContain("stackTrace");
        body.Should().NotContain("innerException");
        body.Should().NotContain("ArgumentException");
        body.Should().NotContain("NullReferenceException");
    }
}
