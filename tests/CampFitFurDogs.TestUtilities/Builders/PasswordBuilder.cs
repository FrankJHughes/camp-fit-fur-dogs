using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class PasswordBuilder
    : TestDataBuilderBase<PasswordBuilder, PasswordHash>
{
    private string _plaintext = PasswordFixtures.Plain;

    public PasswordBuilder WithPlaintext(string value) => With(b => b._plaintext = value);

    public override PasswordHash Build() => PasswordHash.Create(_plaintext);

    public string BuildPlain() => _plaintext;
}
