using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Customers.Exceptions;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class CustomerIdTests
{
    [Fact]
    public void New_returns_non_default_id()
    {
        var id = CustomerId.New();

        id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void From_wraps_given_guid()
    {
        var guid = Guid.NewGuid();

        var id = CustomerId.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_with_empty_guid_throws()
    {
        Action act = () => CustomerId.From(Guid.Empty);

        act.Should().Throw<InvalidCustomerIdException>();
    }

    [Fact]
    public void Two_ids_with_same_guid_are_equal()
    {
        var guid = Guid.NewGuid();

        CustomerId.From(guid).Should().Be(CustomerId.From(guid));
    }

    [Fact]
    public void Two_ids_with_different_guids_are_not_equal()
    {
        CustomerId.New().Should().NotBe(CustomerId.New());
    }
}
