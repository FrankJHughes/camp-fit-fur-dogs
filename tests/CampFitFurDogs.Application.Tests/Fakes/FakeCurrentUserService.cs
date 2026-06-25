using Frank.Abstractions.Identity;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeCurrentUser(Guid currentUserId) : ICurrentUser
{
    public Guid? Id { get; } = currentUserId;

    public bool IsAuthenticated => true;
}
