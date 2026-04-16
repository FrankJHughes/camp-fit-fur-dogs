using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests;

public static class ApiTestHelpers
{
    public sealed record OwnerResponse(Guid CustomerId);
    public sealed record RegisterDogResponse(Guid DogId);

    public static async Task<Guid> CreateOwnerAsync(HttpClient client)
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"owner-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<OwnerResponse>();
        return body!.CustomerId;
    }

    public static async Task<Guid> CreateAndSetOwnerAsync(
        HttpClient client, TestCurrentUserService testUserService)
    {
        var ownerId = await CreateOwnerAsync(client);
        testUserService.CurrentUserId = ownerId;
        return ownerId;
    }

    public static async Task<Guid> RegisterDogAsync(
        HttpClient client, TestCurrentUserService testUserService, Guid ownerId)
    {
        testUserService.CurrentUserId = ownerId;

        var request = new
        {
            Name = "Biscuit",
            Breed = "Golden Retriever",
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterDogResponse>();
        return body!.DogId;
    }
}
