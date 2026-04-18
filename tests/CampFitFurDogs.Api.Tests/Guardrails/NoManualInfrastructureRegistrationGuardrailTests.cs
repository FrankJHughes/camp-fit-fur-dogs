using FluentAssertions;
using CampFitFurDogs.Api.Tests.Guardrails.Architecture;
using CampFitFurDogs.Api.Tests;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoManualInfrastructureRegistrationGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public NoManualInfrastructureRegistrationGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void Should_Not_Have_Manual_Infrastructure_Registrations()
    {
        var infraAssembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;

        var infraTypes = DiRegistrationScanner.FindTypesWithInterfaces(
            infraAssembly,
            t =>
                t.IsClass &&
                !t.IsAbstract &&
                (
                    t.Name.EndsWith("Repository") ||
                    t.Name.EndsWith("Provider") ||
                    t.Name.EndsWith("Service")
                ));

        foreach (var (type, iface) in infraTypes)
        {
            iface.Should().NotBeNull();
            var registrations = DiRegistrationScanner.GetRegistrations(Factory.Services, iface!);

            registrations.Should().ContainSingle(
                $"{type.Name} must be registered exactly once via Scrutor, not manually");
        }
    }
}
