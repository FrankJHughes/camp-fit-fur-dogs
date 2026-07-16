namespace Frank.Core.Domain.Tests.Fakes;

public sealed class FakeEntity : Entity<FakeEntityId>
{
    public FakeEntity(FakeEntityId id) : base()
    {
        Id = id;
    }
}
