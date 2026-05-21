using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class EmailBuilder 
    : TestDataBuilderBase<EmailBuilder, Email>
{
    private string _value = $"test-{Guid.NewGuid()}@example.com";

    public EmailBuilder WithValue(string value) => With(b => b._value = value);

    public override Email Build() => Email.From(_value);
}
