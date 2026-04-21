using Xunit;
using SharedKernel.Domain;

namespace SharedKernel.Tests;

public class ValueObjectTests
{
    [Fact]
    public void Value_objects_with_same_components_are_equal()
    {
        var a = new TestValueObject("123 Main St", "Springfield");
        var b = new TestValueObject("123 Main St", "Springfield");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Value_objects_with_different_components_are_not_equal()
    {
        var a = new TestValueObject("123 Main St", "Springfield");
        var b = new TestValueObject("456 Oak Ave", "Springfield");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Value_object_is_not_equal_to_null()
    {
        var vo = new TestValueObject("123 Main St", "Springfield");

        Assert.False(vo.Equals(null));
    }

    [Fact]
    public void Same_components_produce_same_hash_code()
    {
        var a = new TestValueObject("123 Main St", "Springfield");
        var b = new TestValueObject("123 Main St", "Springfield");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equality_operator_returns_true_for_same_components()
    {
        var a = new TestValueObject("123 Main St", "Springfield");
        var b = new TestValueObject("123 Main St", "Springfield");

        Assert.True(a == b);
    }

    [Fact]
    public void Inequality_operator_returns_true_for_different_components()
    {
        var a = new TestValueObject("123 Main St", "Springfield");
        var b = new TestValueObject("456 Oak Ave", "Shelbyville");

        Assert.True(a != b);
    }
}
