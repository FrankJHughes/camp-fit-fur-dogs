using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

[Collection("API With Postgres")]
public class NoManualInfrastructureRegistrationGuardrailTests
    : ApiWithPostgresTestBase
{
    public NoManualInfrastructureRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture) { }

    [Fact]
    public void Should_Not_Have_Manual_Infrastructure_Registrations()
    {
        var infraAssembly = typeof(CampFitFurDogs.Infrastructure.ServiceCollectionExtensions).Assembly;

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
