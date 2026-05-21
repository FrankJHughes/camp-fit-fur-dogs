using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class LastNameBuilder
    : TestDataBuilderBase<LastNameBuilder, LastName>
{
    private string _value = "Hughes";

    public LastNameBuilder WithValue(string value)
        => With(b => b._value = value);

    public override LastName Build()
        => LastName.From(_value);
}
