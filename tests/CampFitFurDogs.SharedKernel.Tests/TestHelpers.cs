using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.SharedKernel.Tests;

public class TestEntity : Entity<Guid>
{
    public TestEntity(Guid id) : base(id) { }
}

public class OtherTestEntity : Entity<Guid>
{
    public OtherTestEntity(Guid id) : base(id) { }
}

public record TestDomainEvent(string Description = "something happened") : IDomainEvent;

public class TestAggregate : AggregateRoot<Guid>
{
    public TestAggregate(Guid id) : base(id) { }

    public void DoSomething() => RaiseDomainEvent(new TestDomainEvent());
}

public class TestValueObject : ValueObject
{
    public string Street { get; }
    public string City { get; }

    public TestValueObject(string street, string city)
    {
        Street = street;
        City = city;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
    }
}