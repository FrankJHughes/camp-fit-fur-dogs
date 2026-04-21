using SharedKernel.Abstractions;

namespace CampFitFurDogs.Api.Tests;

public sealed class TestCurrentUser : ICurrentUserService
{
    public Guid CurrentUserId { get; set; } = Guid.NewGuid();
}
