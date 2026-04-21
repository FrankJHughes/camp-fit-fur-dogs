using Xunit;
using SharedKernel.Domain;

namespace SharedKernel.Tests;

public class EntityTests
{
    [Fact]
    public void Entities_with_same_id_are_equal()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity(id);
        var b = new TestEntity(id);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Entities_with_different_ids_are_not_equal()
    {
        var a = new TestEntity(Guid.NewGuid());
        var b = new TestEntity(Guid.NewGuid());

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Entity_is_not_equal_to_null()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.False(entity.Equals(null));
    }

    [Fact]
    public void Entities_of_different_types_with_same_id_are_not_equal()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);
        var other = new OtherTestEntity(id);

        Assert.False(entity.Equals(other));
    }

    [Fact]
    public void Same_id_produces_same_hash_code()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity(id);
        var b = new TestEntity(id);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equality_operator_returns_true_for_same_id()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity(id);
        var b = new TestEntity(id);

        Assert.True(a == b);
    }

    [Fact]
    public void Inequality_operator_returns_true_for_different_ids()
    {
        var a = new TestEntity(Guid.NewGuid());
        var b = new TestEntity(Guid.NewGuid());

        Assert.True(a != b);
    }
}
