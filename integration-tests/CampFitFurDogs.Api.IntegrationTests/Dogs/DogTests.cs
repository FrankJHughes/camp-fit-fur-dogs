using System.Net.Http.Json;
using Xunit;

namespace CampFitFurDogs.Api.IntegrationTests.Dogs;

public class DogTests : ApiTestBase
{
    private static readonly Guid PlaceholderUserId =
        Guid.Parse("00000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task Can_Register_And_Retrieve_Dog_Profile()
    {
        // STEP 1 — Ensure the placeholder user exists as a customer
        // var email = $"preview-{Guid.NewGuid()}@example.com";

        // var createCustomerResponse = await Client.PostAsJsonAsync("/api/customers", new
        // {
        //     // id = PlaceholderUserId,   // <-- IMPORTANT
        //     firstName = "Preview",
        //     lastName = "User",
        //     email,
        //     phone = "555-0000",
        //     password = "P@ssw0rd!"
        // });

        // Ignore 409/400 — customer may already exist
        // if (!createCustomerResponse.IsSuccessStatusCode &&
        //     createCustomerResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
        // {
        //     createCustomerResponse.EnsureSuccessStatusCode();
        // }

        // STEP 2 — Register a dog
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

        var createdDog = await registerResponse.Content.ReadFromJsonAsync<RegisterDogResponse>();
        Assert.NotNull(createdDog);

        // STEP 3 — Retrieve dog profile
        var profileResponse = await Client.GetAsync($"/api/dogs/{createdDog!.DogId}");
        profileResponse.EnsureSuccessStatusCode();

        var profile = await profileResponse.Content.ReadFromJsonAsync<DogProfileDto>();

        // STEP 4 — Assertions
        Assert.NotNull(profile);
        Assert.Equal(createdDog.DogId, profile!.Id);
        Assert.Equal(dogName, profile.Name);
        Assert.Equal(breed, profile.Breed);
        Assert.Equal(sex, profile.Sex);
        Assert.Equal(PlaceholderUserId, profile.OwnerId);
    }

    public record RegisterDogResponse(Guid DogId);
    public record DogProfileDto(Guid Id, string Name, string Breed, string Sex, string DateOfBirth, Guid OwnerId);
}
