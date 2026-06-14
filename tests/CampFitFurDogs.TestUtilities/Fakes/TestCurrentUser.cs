using Frank.Abstractions;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class TestCurrentUser : ICurrentUser
{
    public Guid Id { get; set; }

    public TestCurrentUser(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }
}
