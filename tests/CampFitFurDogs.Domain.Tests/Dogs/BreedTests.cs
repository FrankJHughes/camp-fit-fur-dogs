using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class BreedTests
{
    [Fact]
    public void Create_TrimmedValue()
    {
        var breed = Breed.Create("  Golden Retriever  ");
        Assert.Equal("Golden Retriever", breed.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespace_Throws(string? value)
    {
        Assert.Throws<ArgumentException>(() => Breed.Create(value!));
    }

    [Fact]
    public void EqualBreeds_AreEqual()
    {
        Assert.Equal(Breed.Create("Poodle"), Breed.Create("Poodle"));
    }
}
