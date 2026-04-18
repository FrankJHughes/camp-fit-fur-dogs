using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class InfrastructureServiceRegistrationGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public InfrastructureServiceRegistrationGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void Should_Register_All_Services()
    {
        var assembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;

        var serviceTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("Service"))
            .ToList();

        serviceTypes.Should().NotBeEmpty("services must exist to test");

        foreach (var serviceType in serviceTypes)
        {
            var iface = serviceType.GetInterfaces().FirstOrDefault();
            iface.Should().NotBeNull($"Service {serviceType.Name} must implement an interface");

            var resolved = GetAll(iface);
            resolved.Should().NotBeEmpty($"Service {serviceType.Name} must be registered in DI");
        }
    }
}
