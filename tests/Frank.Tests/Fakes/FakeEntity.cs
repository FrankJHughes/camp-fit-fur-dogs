using Frank.Domain;
using Frank.Tests.Fakes;

namespace Frank.Tests.Domain;

public sealed partial class EntityTests
{
    private sealed class FakeEntity : Entity<FakeEntityId>
    {
        public FakeEntity(FakeEntityId id) : base()
        {
            Id = id;
        }
    }
}
