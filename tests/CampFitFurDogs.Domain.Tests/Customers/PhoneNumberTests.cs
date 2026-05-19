using FluentAssertions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class PhoneNumberTests
{
    [Fact]
    public void From_with_valid_phone_succeeds()
    {
        var phone = PhoneNumber.From("916-555-1234");
        phone.Value.Should().Be("+19165551234");
    }

    [Fact]
    public void From_trims_whitespace()
    {
        var phone = PhoneNumber.From("   916-555-1234   ");
        phone.Value.Should().Be("+19165551234");
    }

    [Fact]
    public void From_with_empty_string_throws()
    {
        Action act = () => PhoneNumber.From("");
        act.Should().Throw<InvalidPhoneNumberException>()
           .WithMessage("*empty*");
    }

    [Fact]
    public void From_with_whitespace_throws()
    {
        Action act = () => PhoneNumber.From("   ");
        act.Should().Throw<InvalidPhoneNumberException>()
           .WithMessage("*empty*");
    }
}
