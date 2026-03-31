using Xunit;
using CampFitFurDogs.Domain.Guardians;

namespace CampFitFurDogs.Domain.Tests.Guardians;

public class GuardianIdTests
{
    [Fact]
    public void New_returns_non_default_id()
    {
        var id = GuardianId.New();
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void From_wraps_given_guid()
    {
        var guid = Guid.NewGuid();
        var id = GuardianId.From(guid);
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_with_empty_guid_throws()
    {
        Assert.Throws<ArgumentException>(() => GuardianId.From(Guid.Empty));
    }

    [Fact]
    public void Two_ids_with_same_guid_are_equal()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(GuardianId.From(guid), GuardianId.From(guid));
    }

    [Fact]
    public void Two_ids_with_different_guids_are_not_equal()
    {
        Assert.NotEqual(GuardianId.New(), GuardianId.New());
    }

    [Fact]
    public void Equality_operator_returns_true_for_same_guid()
    {
        var guid = Guid.NewGuid();
        Assert.True(GuardianId.From(guid) == GuardianId.From(guid));
    }

    [Fact]
    public void Inequality_operator_returns_true_for_different_guids()
    {
        Assert.True(GuardianId.New() != GuardianId.New());
    }
}
