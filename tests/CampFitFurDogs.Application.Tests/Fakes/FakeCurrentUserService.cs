using Frank.Abstractions;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeCurrentUserService(Guid currentUserId) : ICurrentUserService
{
    public Guid CurrentUserId { get; } = currentUserId;
}
