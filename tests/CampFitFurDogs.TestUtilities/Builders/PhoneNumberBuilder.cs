using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class PhoneNumberBuilder
    : TestDataBuilderBase<PhoneNumberBuilder, PhoneNumber>
{
    private string _digits = PhoneNumberFixtures.Valid;

    public PhoneNumberBuilder WithDigits(string digits) => With(b => b._digits = digits);

    public override PhoneNumber Build() => PhoneNumber.From(_digits);
}
