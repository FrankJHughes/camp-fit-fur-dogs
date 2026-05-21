using SharedKernel.Abstractions;

namespace CampFitFurDogs.Integration.Tests;

public sealed class TestCurrentUser : ICurrentUserService
{
    public Guid CurrentUserId { get; set; } = Guid.NewGuid();
}
