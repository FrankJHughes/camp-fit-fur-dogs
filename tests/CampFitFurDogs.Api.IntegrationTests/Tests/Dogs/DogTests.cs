using System.Net.Http.Json;
using Xunit;

namespace CampFitFurDogs.Api.IntegrationTests.Tests.Dogs;

public class DogTests : ApiTestBase
{
    [Fact]
    public async Task Can_Register_And_Retrieve_Dog_Profile()
    {
        // Arrange
        var dogName = $"TestDog-{Guid.NewGuid()}";
        var breed = "Golden Retriever";
        var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)).ToString("yyyy-MM-dd");
        var sex = "Male";

        var registerResponse = await Client.PostAsJsonAsync("/api/dogs", new
        {
            name = dogName,
            breed,
            dateOfBirth = dob,
            sex
        });

        registerResponse.EnsureSuccessStatusCode();

        // Extract DogId from response
        var created = await registerResponse.Content.ReadFromJsonAsync<RegisterDogResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.DogId);

        // Act — fetch the dog profile
        var profileResponse = await Client.GetAsync($"/api/dogs/{created.DogId}");
        profileResponse.EnsureSuccessStatusCode();

        var profile = await profileResponse.Content.ReadFromJsonAsync<DogProfileDto>();

        // Assert — the preview DB is being used and data is correct
        Assert.NotNull(profile);
        Assert.Equal(created.DogId, profile!.Id);
        Assert.Equal(dogName, profile.Name);
        Assert.Equal(breed, profile.Breed);
        Assert.Equal(sex, profile.Sex);
    }

    public record RegisterDogResponse(Guid DogId);

    public record DogProfileDto(
        Guid Id,
        string Name,
        string Breed,
        string Sex,
        string DateOfBirth,
        Guid OwnerId
    );
}
