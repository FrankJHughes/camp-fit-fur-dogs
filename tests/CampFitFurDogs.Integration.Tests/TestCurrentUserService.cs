using Frank.Abstractions.Identity;

namespace CampFitFurDogs.Integration.Tests;

public sealed class TestCurrentUser : ICurrentUser
{
    public Guid? Id { get; set; } = Guid.NewGuid();

    public bool IsAuthenticated => true;
}
