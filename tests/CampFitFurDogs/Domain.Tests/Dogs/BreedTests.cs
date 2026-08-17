using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class BreedTests
{
    [Fact]
    public void Create_trims_value()
    {
        var breed = Breed.Create("  Golden Retriever  ");
        breed.Value.Should().Be("Golden Retriever");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_with_empty_or_whitespace_throws(string? value)
    {
        Action act = () => Breed.Create(value!);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*required*");
    }

    [Fact]
    public void Equal_breeds_are_equal()
    {
        var a = Breed.Create("Poodle");
        var b = Breed.Create("Poodle");

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
