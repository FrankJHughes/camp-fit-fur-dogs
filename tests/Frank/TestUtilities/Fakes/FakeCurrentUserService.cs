using Frank.Identity.Application.Abstractions.Users;

namespace Frank.TestUtilties.Fakes;

public class FakeCurrentUser(Guid currentUserId) : ICurrentUser
{
    public Guid? Id { get; } = currentUserId;
    public string? Name { get; } = default!;

    public bool IsAuthenticated => true;
}
