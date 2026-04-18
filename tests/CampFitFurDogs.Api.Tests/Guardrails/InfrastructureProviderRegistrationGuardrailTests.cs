using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class InfrastructureProviderRegistrationGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public InfrastructureProviderRegistrationGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void Should_Register_All_Providers()
    {
        var assembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;

        var providerTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("Provider"))
            .ToList();

        if (!providerTypes.Any())
            return;

        providerTypes.Should().NotBeEmpty("providers must exist to test");

        foreach (var providerType in providerTypes)
        {
            var iface = providerType.GetInterfaces().FirstOrDefault();
            iface.Should().NotBeNull($"Provider {providerType.Name} must implement an interface");

            var resolved = GetAll(iface);
            resolved.Should().NotBeEmpty($"Provider {providerType.Name} must be registered in DI");
        }
    }
}
