namespace Frank.Core.Domain.Tests.Fakes;

public sealed class FakeAggregateId : AggregateId
{
    private FakeAggregateId(Guid value) : base(value) { }

    public static FakeAggregateId New() => new(Guid.NewGuid());
}
