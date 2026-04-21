using FluentAssertions;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class InfrastructureRegistrationGuardrailTests : ApiTestBase
{
    public InfrastructureRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

    [Theory]
    [InlineData("Repository")]
    [InlineData("Reader")]
    [InlineData("Provider")]
    [InlineData("Service")]
    public void All_Types_With_Suffix_Are_Registered(string suffix)
    {
        var assembly = typeof(AssemblyMarker).Assembly;

        var types = assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(suffix))
            .ToList();

        foreach (var type in types)
        {
            var iface = type.GetInterfaces().FirstOrDefault();
            iface.Should().NotBeNull();

            var resolved = GetAll(iface);
            resolved.Should().NotBeEmpty();
        }
    }
}
