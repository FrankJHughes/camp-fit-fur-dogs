using Frank.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class ApiRequestBuilder
{
    public static object User(
        string? email = null,
        string? phone = null,
        string? password = null)
    {
        return new UserBuilder()
            .WithEmail(email ?? $"api-{Guid.NewGuid()}@example.com")
            .WithPhone(phone ?? "916-555-1234")
            .BuildApiRequest();
    }

    public static object Dog(
        string name = "Biscuit",
        string breed = "Golden Retriever",
        string dob = "2022-06-15",
        string sex = "Female")
    {
        return new
        {
            Name = name,
            Breed = breed,
            DateOfBirth = dob,
            Sex = sex
        };
    }
}
