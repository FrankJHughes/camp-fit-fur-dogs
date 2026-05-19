using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class DogFixtures
{
    public const string DefaultName = "Biscuit";
    public const string DefaultBreed = "Golden Retriever";

    public static readonly DateOnly Dob = new DateOnly(2022, 6, 15);
    public static readonly Sex Sex = Sex.Female;

    public static DogName DogName => DogName.Create(DefaultName);
    public static Breed DogBreed => Breed.Create(DefaultBreed);
}
