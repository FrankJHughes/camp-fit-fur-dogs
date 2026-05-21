using FluentAssertions;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogIdTests
{
    [Fact]
    public void New_generates_non_empty_guid()
    {
        var id = DogId.New();
        id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void From_wraps_existing_guid()
    {
        var guid = Guid.NewGuid();
        var id = DogId.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_with_empty_guid_throws()
    {
        Action act = () => DogId.From(Guid.Empty);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*empty*");
    }

    [Fact]
    public void Equal_ids_are_equal()
    {
        var guid = Guid.NewGuid();

        var a = DogId.From(guid);
        var b = DogId.From(guid);

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
