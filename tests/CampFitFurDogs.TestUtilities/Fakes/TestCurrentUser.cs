using Frank.Abstractions.Identity;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class TestCurrentUser : ICurrentUser
{
    public Guid? Id { get; set; }

    public bool IsAuthenticated => true;

    public string? Name => throw new NotImplementedException();

    public TestCurrentUser(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }
}
