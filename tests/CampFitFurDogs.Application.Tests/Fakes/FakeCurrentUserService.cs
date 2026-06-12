using Frank.Abstractions;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeCurrentUserService(Guid currentUserId) : ICurrentUser
{
    public Guid Id { get; } = currentUserId;
}
