using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fakes;
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
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        var response = await client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<OwnerResponse>();
        return body!.CustomerId;
    }

    public static async Task<Guid> CreateAndSetOwnerAsync(
        HttpClient client,
        TestCurrentUser testUser)
    {
        var ownerId = await CreateOwnerAsync(client);
        testUser.CurrentUserId = ownerId;
        return ownerId;
    }

    public static async Task<Guid> RegisterDogAsync(
        HttpClient client,
        TestCurrentUser testUser,
        Guid ownerId,
        string name = "Biscuit",
        string breed = "Golden Retriever")
    {
        // The API uses the CURRENT USER as the owner
        testUser.CurrentUserId = ownerId;

        // Build ONLY the API request fields (no OwnerId)
        var request = new DogBuilder()
            .WithName(name)
            .WithBreed(breed)
            .BuildApiRequest();

        var response = await client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterDogResponse>();
        return body!.DogId;
    }
}
