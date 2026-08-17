namespace Frank.Core.Domain.Tests.Fakes;

public sealed class FakeValueObject : ValueObject
{
    public string Value { get; }

    public FakeValueObject(string value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
