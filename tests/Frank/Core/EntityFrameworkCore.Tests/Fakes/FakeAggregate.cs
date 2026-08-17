using Frank.Core.Domain;

namespace Frank.Core.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeAggregate : AggregateRoot<FakeAggregateId>
{
    public string Name { get; private set; } = string.Empty;

    public FakeAggregate() : base(new FakeAggregateId(Guid.NewGuid())) { }
}
