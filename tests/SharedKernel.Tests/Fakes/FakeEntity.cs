using SharedKernel.Domain;
using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Domain;

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
