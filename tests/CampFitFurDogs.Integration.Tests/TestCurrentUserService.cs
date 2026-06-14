using Frank.Abstractions;

namespace CampFitFurDogs.Integration.Tests;

public sealed class TestCurrentUser : ICurrentUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
