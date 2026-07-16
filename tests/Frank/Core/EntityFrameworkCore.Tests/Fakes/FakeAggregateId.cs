using Frank.Core.Domain;

namespace Frank.Core.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeAggregateId : AggregateId
{
    public FakeAggregateId(Guid value) : base(value) { }
}
