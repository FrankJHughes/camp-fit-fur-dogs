using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class DogFixtures
{
    // ------------------------------------------------------------
    // Default values for tests
    // ------------------------------------------------------------

    public const string DefaultName = "Biscuit";
    public const string DefaultBreed = "Golden Retriever";

    public static readonly DateOnly Dob = new(2022, 6, 15);
    public static readonly Sex Sex = Sex.Female;

    // ------------------------------------------------------------
    // Prebuilt domain value objects (validated)
    // ------------------------------------------------------------

    public static DogName DefaultDogName => DogName.Create(DefaultName);
    public static Breed DefaultDogBreed => Breed.Create(DefaultBreed);
}
