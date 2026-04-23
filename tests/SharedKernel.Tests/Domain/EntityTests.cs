using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Domain;

public sealed partial class EntityTests
{

    [Fact]
    public void Entities_With_Same_Id_Are_Equal()
    {
        var id = FakeEntityId.New();
        var e1 = new FakeEntity(id);
        var e2 = new FakeEntity(id);

        e1.Equals(e2).Should().BeTrue();
    }

    [Fact]
    public void Entities_With_Different_Ids_Are_Not_Equal()
    {
        var e1 = new FakeEntity(FakeEntityId.New());
        var e2 = new FakeEntity(FakeEntityId.New());

        e1.Should().NotBe(e2);
        e1.Equals(e2).Should().BeFalse();
    }

    [Fact]
    public void Entity_Equals_Returns_False_When_Comparing_To_Null()
    {
        var e1 = new FakeEntity(FakeEntityId.New());

        e1.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void HashCode_Is_Based_On_Id()
    {
        var id = FakeEntityId.New();
        var e1 = new FakeEntity(id);
        var e2 = new FakeEntity(id);

        e1.GetHashCode().Should().Be(e2.GetHashCode());
    }

    [Fact]
    public void Reference_Equality_Does_Not_Override_Identity_Equality()
    {
        var id = FakeEntityId.New();
        var e1 = new FakeEntity(id);
        var e2 = new FakeEntity(id);

        ReferenceEquals(e1, e2).Should().BeFalse();
        e1.Equals(e2).Should().BeTrue();
    }
}
