using Frank.Domain;

namespace Frank.Infrastructure.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeAggregateId : AggregateId
{
    public FakeAggregateId(Guid value) : base(value) { }
}
