using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Helpers.Dogs;

public static class DogHelper
{
    public static async Task<Guid> RegisterDogAsync(HttpClient client, string name, string breed)
    {
        var request = new
        {
            Name = name,
            Breed = breed,
            DateOfBirth = "2022-06-15",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var path = response.Headers.Location!.OriginalString;
        var dogId = path[(1 + path.LastIndexOf('/'))..];
        return Guid.Parse(dogId);
    }

}
