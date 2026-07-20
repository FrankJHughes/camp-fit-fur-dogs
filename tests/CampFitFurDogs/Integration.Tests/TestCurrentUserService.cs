using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Integration.Tests;

public sealed class TestCurrentUser : ICurrentUser
{
    public Guid? Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } = default!;

    public bool IsAuthenticated => true;
}
