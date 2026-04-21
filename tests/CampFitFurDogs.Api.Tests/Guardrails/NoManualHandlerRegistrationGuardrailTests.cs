using FluentAssertions;
using CampFitFurDogs.Api.Tests;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoManualHandlerRegistrationGuardrailTests
    : ApiTestBase
{
    public NoManualHandlerRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

    [Fact]
    public void Should_Not_Have_Manual_Handler_Registrations()
    {
        var appAssembly = typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly;

        var handlers = DiRegistrationScanner.FindTypesWithInterfaces(
            appAssembly,
            t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition().Name.StartsWith("ICommandHandler") ||
                        i.GetGenericTypeDefinition().Name.StartsWith("IQueryHandler") ||
                        i.GetGenericTypeDefinition().Name.StartsWith("IDomainEventHandler")
                    )
                ));

        foreach (var (type, iface) in handlers)
        {
            iface.Should().NotBeNull();
            var registrations = DiRegistrationScanner.GetRegistrations(Factory.Services, iface!);

            registrations.Should().ContainSingle(
                $"{type.Name} must be registered exactly once via Scrutor, not manually");
        }
    }
}
