using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class EmailTests
{
    [Fact]
    public void From_with_valid_email_succeeds()
    {
        var email = Email.From("frank@example.com");
        Assert.Equal("frank@example.com", email.Value);
    }

    [Fact]
    public void From_normalizes_to_lowercase()
    {
        var email = Email.From("Frank@Example.COM");
        Assert.Equal("frank@example.com", email.Value);
    }

    [Fact]
    public void From_trims_whitespace()
    {
        var email = Email.From("  frank@example.com  ");
        Assert.Equal("frank@example.com", email.Value);
    }

    [Fact]
    public void From_with_empty_string_throws()
    {
        Assert.Throws<ArgumentException>(() => Email.From(""));
    }

    [Fact]
    public void From_with_whitespace_throws()
    {
        Assert.Throws<ArgumentException>(() => Email.From("   "));
    }

    [Fact]
    public void From_with_no_at_sign_throws()
    {
        Assert.Throws<ArgumentException>(() => Email.From("not-an-email"));
    }

    [Fact]
    public void Two_emails_with_same_value_are_equal()
    {
        Assert.Equal(Email.From("a@b.com"), Email.From("a@b.com"));
    }
}
