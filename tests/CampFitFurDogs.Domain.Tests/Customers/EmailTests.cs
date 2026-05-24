using FluentAssertions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;
using CampFitFurDogs.Domain.Customers.Exceptions;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class EmailTests
{
    [Fact]
    public void From_with_valid_email_succeeds()
    {
        var email = Email.From("frank@example.com");
        email.Value.Should().Be("frank@example.com");
    }

    [Fact]
    public void From_normalizes_to_lowercase()
    {
        var email = Email.From("Frank@Example.COM");
        email.Value.Should().Be("frank@example.com");
    }

    [Fact]
    public void From_trims_whitespace()
    {
        var email = Email.From("  frank@example.com  ");
        email.Value.Should().Be("frank@example.com");
    }

    [Fact]
    public void From_with_empty_string_throws()
    {
        Action act = () => Email.From("");
        act.Should().Throw<InvalidEmailException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void From_with_whitespace_throws()
    {
        Action act = () => Email.From("   ");
        act.Should().Throw<InvalidEmailException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void From_with_no_at_sign_throws()
    {
        Action act = () => Email.From("not-an-email");
        act.Should().Throw<InvalidEmailException>()
            .WithMessage("*format*");
    }

    [Fact]
    public void Two_emails_with_same_value_are_equal()
    {
        var a = Email.From("a@b.com");
        var b = Email.From("a@b.com");

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
