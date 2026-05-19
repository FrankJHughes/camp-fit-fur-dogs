using FluentAssertions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class CustomerTests
{
    private static FirstName ValidFirstName => FirstName.From(NameFixtures.DefaultFirst);
    private static LastName ValidLastName => LastName.From(NameFixtures.DefaultLast);
    private static Email ValidEmail => Email.From(EmailFixtures.Default);
    private static PhoneNumber ValidPhone => PhoneNumber.From(PhoneNumberFixtures.Valid);
    private static PasswordHash ValidHash => PasswordFixtures.Hash();

    [Fact]
    public void Create_returns_customer_with_correct_properties()
    {
        var customer = Customer.Create(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            ValidHash);

        customer.FirstName.Should().Be(ValidFirstName);
        customer.LastName.Should().Be(ValidLastName);
        customer.Email.Should().Be(ValidEmail);
        customer.Phone.Should().Be(ValidPhone);
    }

    [Fact]
    public void Create_assigns_new_customer_id()
    {
        var customer = Customer.Create(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            ValidHash);

        customer.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_with_null_first_name_throws()
    {
        Action act = () => Customer.Create(
            null!,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            ValidHash);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_with_empty_first_name_throws()
    {
        Action act = () => Customer.Create(
            FirstName.From(""),
            ValidLastName,
            ValidEmail,
            ValidPhone,
            ValidHash);

        act.Should().Throw<InvalidFirstNameException>();
    }

    [Fact]
    public void Create_with_null_last_name_throws()
    {
        Action act = () => Customer.Create(
            ValidFirstName,
            null!,
            ValidEmail,
            ValidPhone,
            ValidHash);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_with_empty_last_name_throws()
    {
        Action act = () => Customer.Create(
            ValidFirstName,
            LastName.From(""),
            ValidEmail,
            ValidPhone,
            ValidHash);

        act.Should().Throw<InvalidLastNameException>();
    }
}
