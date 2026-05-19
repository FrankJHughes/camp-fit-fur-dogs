namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class ApiRequestBuilder
{
    public static object Customer(
        string? email = null,
        string? phone = null,
        string? password = null)
    {
        return new CustomerBuilder()
            .WithEmail(email ?? $"api-{Guid.NewGuid()}@example.com")
            .WithPhone(phone ?? "916-555-1234")
            .WithPassword(password ?? "SuperSecure123!")
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
