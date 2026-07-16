using Frank.Identity.Application.Abstractions;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class TestCurrentUser : ICurrentUser
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }

    public bool IsAuthenticated => true;

    public TestCurrentUser(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }
}
