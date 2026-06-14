using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests;

public static class ApiTestHelpers
{
    private sealed record DogResponse(Guid DogId);

    // ------------------------------------------------------------
    // AUTHENTICATE USING REAL AUTH CALLBACK PIPELINE
    // ------------------------------------------------------------
    public static async Task AuthenticateAsync(
        HttpClient client,
        string code = "test-code")
    {
        var response = await client.GetAsync($"/api/auth/callback?code={code}");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect); // 302
    }

    // ------------------------------------------------------------
    // REGISTER DOG (OWNER IS IMPLICIT FROM SESSION COOKIE)
    // ------------------------------------------------------------
    public static async Task<Guid> RegisterDogAsync(
        HttpClient client,
        string name,
        string breed)
    {
        var request = new
        {
            Name = name,
            Breed = breed,
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);
        response.EnsureSuccessStatusCode();

        var dog = await response.Content.ReadFromJsonAsync<DogResponse>();
        return dog!.DogId;
    }
}
