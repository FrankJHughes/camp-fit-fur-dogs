using Frank.Domain.Users;
using Frank.Domain.Users.Exceptions;

namespace Frank.Tests.Domain.Users;

public class UserIdTests
{
    [Fact]
    public void New_returns_non_default_id()
    {
        var id = UserId.New();

        id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void From_wraps_given_guid()
    {
        var guid = Guid.NewGuid();

        var id = UserId.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_with_empty_guid_throws()
    {
        Action act = () => UserId.From(Guid.Empty);

        act.Should().Throw<InvalidUserIdException>();
    }

    [Fact]
    public void Two_ids_with_same_guid_are_equal()
    {
        var guid = Guid.NewGuid();

        UserId.From(guid).Should().Be(UserId.From(guid));
    }

    [Fact]
    public void Two_ids_with_different_guids_are_not_equal()
    {
        UserId.New().Should().NotBe(UserId.New());
    }
}
