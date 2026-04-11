using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogNameTests
{
    [Fact]
    public void Create_TrimmedValue()
    {
        var name = DogName.Create("  Biscuit  ");
        Assert.Equal("Biscuit", name.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespace_Throws(string? value)
    {
        Assert.Throws<ArgumentException>(() => DogName.Create(value!));
    }

    [Fact]
    public void EqualNames_AreEqual()
    {
        Assert.Equal(DogName.Create("Biscuit"), DogName.Create("Biscuit"));
    }
}
