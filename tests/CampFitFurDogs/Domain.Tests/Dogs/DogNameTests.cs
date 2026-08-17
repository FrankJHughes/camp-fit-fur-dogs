using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogNameTests
{
    [Fact]
    public void Create_trims_value()
    {
        var name = DogName.Create("  Biscuit  ");
        name.Value.Should().Be("Biscuit");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_with_empty_or_whitespace_throws(string? value)
    {
        Action act = () => DogName.Create(value!);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*required*");
    }

    [Fact]
    public void Equal_names_are_equal()
    {
        var a = DogName.Create("Biscuit");
        var b = DogName.Create("Biscuit");

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
