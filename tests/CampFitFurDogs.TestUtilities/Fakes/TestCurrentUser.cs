using Frank.Abstractions;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class TestCurrentUser : ICurrentUserService
{
    public Guid CurrentUserId { get; set; } = Guid.NewGuid();
}
