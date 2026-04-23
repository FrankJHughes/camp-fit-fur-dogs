using FluentAssertions;
using SharedKernel.Domain;
using Xunit;

namespace SharedKernel.Tests.Domain;

public sealed class ValueObjectTests
{
    private sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    [Fact]
    public void ValueObjects_With_Same_Components_Are_Equal()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(10, "USD");

        m1.Should().Be(m2);
        m1.GetHashCode().Should().Be(m2.GetHashCode());
    }

    [Fact]
    public void ValueObjects_With_Different_Components_Are_Not_Equal()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(20, "USD");

        m1.Should().NotBe(m2);
    }

    [Fact]
    public void ValueObject_Equals_Returns_False_When_Comparing_To_Null()
    {
        var m1 = new Money(10, "USD");

        m1.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_Equality_Is_Structural_Not_Reference()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(10, "USD");

        ReferenceEquals(m1, m2).Should().BeFalse();
        m1.Should().Be(m2);
    }
}
