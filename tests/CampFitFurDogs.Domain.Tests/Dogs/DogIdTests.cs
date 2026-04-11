using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogIdTests
{
    [Fact]
    public void New_GeneratesNonEmptyGuid()
    {
        var id = DogId.New();
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void From_WrapsExistingGuid()
    {
        var guid = Guid.NewGuid();
        var id = DogId.From(guid);
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_EmptyGuid_Throws()
    {
        Assert.Throws<ArgumentException>(() => DogId.From(Guid.Empty));
    }

    [Fact]
    public void EqualIds_AreEqual()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(DogId.From(guid), DogId.From(guid));
    }
}
