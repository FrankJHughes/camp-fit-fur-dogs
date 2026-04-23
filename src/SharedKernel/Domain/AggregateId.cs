using System.Collections.Generic;

namespace SharedKernel.Domain;

public abstract class AggregateId : ValueObject
{
    public Guid Value { get; }

    protected AggregateId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
