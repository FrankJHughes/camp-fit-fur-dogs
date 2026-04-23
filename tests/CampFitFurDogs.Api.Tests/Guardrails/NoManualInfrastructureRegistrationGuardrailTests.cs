using FluentAssertions;
using CampFitFurDogs.Api.Tests;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoManualInfrastructureRegistrationGuardrailTests
    : ApiTestBase
{
    public NoManualInfrastructureRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

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
