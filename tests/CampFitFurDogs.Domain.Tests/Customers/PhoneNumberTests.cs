using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class PhoneNumberTests
{
    [Fact]
    public void From_with_valid_phone_succeeds()
    {
        var phone = PhoneNumber.From("555-1234");
        Assert.Equal("555-1234", phone.Value);
    }

    [Fact]
    public void From_trims_whitespace()
    {
        var phone = PhoneNumber.From("  555-1234  ");
        Assert.Equal("555-1234", phone.Value);
    }

    [Fact]
    public void From_with_empty_string_throws()
    {
        Assert.Throws<ArgumentException>(() => PhoneNumber.From(""));
    }

    [Fact]
    public void From_with_whitespace_throws()
    {
        Assert.Throws<ArgumentException>(() => PhoneNumber.From("   "));
    }
}
