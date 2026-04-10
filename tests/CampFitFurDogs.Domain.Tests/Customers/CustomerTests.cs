using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class CustomerTests
{
    private static Email ValidEmail => Email.From("test@example.com");
    private static PhoneNumber ValidPhone => PhoneNumber.From("555-1234");
    private static PasswordHash ValidHash => PasswordHash.From("hashed");

    [Fact]
    public void Create_returns_customer_with_correct_properties()
    {
        var customer = Customer.Create("Frank", "Hughes", ValidEmail, ValidPhone, ValidHash);

        Assert.Equal("Frank", customer.FirstName);
        Assert.Equal("Hughes", customer.LastName);
        Assert.Equal(ValidEmail, customer.Email);
        Assert.Equal(ValidPhone, customer.Phone);
    }

    [Fact]
    public void Create_assigns_new_customer_id()
    {
        var customer = Customer.Create("Frank", "Hughes", ValidEmail, ValidPhone, ValidHash);
        Assert.NotEqual(Guid.Empty, customer.Id.Value);
    }

    // ── These 4 will be RED ──

    [Fact]
    public void Create_with_null_first_name_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Customer.Create(null!, "Hughes", ValidEmail, ValidPhone, ValidHash));
    }

    [Fact]
    public void Create_with_empty_first_name_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Customer.Create("", "Hughes", ValidEmail, ValidPhone, ValidHash));
    }

    [Fact]
    public void Create_with_null_last_name_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Customer.Create("Frank", null!, ValidEmail, ValidPhone, ValidHash));
    }

    [Fact]
    public void Create_with_empty_last_name_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Customer.Create("Frank", "", ValidEmail, ValidPhone, ValidHash));
    }
}
