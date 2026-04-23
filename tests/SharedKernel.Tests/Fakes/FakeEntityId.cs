using SharedKernel.Domain;

public sealed class FakeEntityId : ValueObject
{
    public Guid Value { get; }

    private FakeEntityId(Guid value)
    {
        Value = value;
    }

    public static FakeEntityId New() => new(Guid.NewGuid());

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
