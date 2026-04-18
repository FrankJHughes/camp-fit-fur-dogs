using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class InfrastructureRegistrationGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public InfrastructureRegistrationGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Theory]
    [InlineData("Repository")]
    [InlineData("Service")]
    [InlineData("Provider")]
    public void Should_Register_All_Infrastructure_Types(string suffix)
    {
        var assembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;

        var types = assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(suffix))
            .ToList();

        if (types.Count == 0) return; // suffix category may not exist yet

        foreach (var type in types)
        {
            var iface = type.GetInterfaces().FirstOrDefault();
            iface.Should().NotBeNull($"{suffix} {type.Name} must implement an interface");

            var resolved = GetAll(iface);
            resolved.Should().NotBeEmpty($"{suffix} {type.Name} must be registered in DI");
        }
    }
}