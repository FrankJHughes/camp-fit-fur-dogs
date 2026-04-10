using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class CustomerIdTests
{
    [Fact]
    public void New_returns_non_default_id()
    {
        var id = CustomerId.New();
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void From_wraps_given_guid()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.From(guid);
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_with_empty_guid_throws()
    {
        Assert.Throws<ArgumentException>(() => CustomerId.From(Guid.Empty));
    }

    [Fact]
    public void Two_ids_with_same_guid_are_equal()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(CustomerId.From(guid), CustomerId.From(guid));
    }

    [Fact]
    public void Two_ids_with_different_guids_are_not_equal()
    {
        Assert.NotEqual(CustomerId.New(), CustomerId.New());
    }
}
