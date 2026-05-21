using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class FirstNameBuilder
    : TestDataBuilderBase<FirstNameBuilder, FirstName>
{
    private string _value = "Frank";

    public FirstNameBuilder WithValue(string value)
        => With(b => b._value = value);

    public override FirstName Build()
        => FirstName.From(_value);
}
