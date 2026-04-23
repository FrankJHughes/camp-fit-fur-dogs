using System.Reflection;
using FluentAssertions;
using SharedKernel.Domain;
using SharedKernel.Abstractions;
using SharedKernel.Events;

using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DomainEventHandlerRegistrationGuardrailTests
    : ApiTestBase
{
    public DomainEventHandlerRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

    [Fact]
    public void Should_Register_All_DomainEventHandlers()
    {
        // Find all concrete classes implementing IDomainEventHandler<T>
        var handlerTypes = typeof(CampFitFurDogs.Application.AssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
            .ToList();

        if(handlerTypes.Count == 0)
        {
            return;
        }

        // handlerTypes.Should().NotBeEmpty("there must be at least one domain event handler to test");

        foreach (var handlerType in handlerTypes)
        {
            // Find the closed generic interface implemented by this handler
            var iface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

            // Ask DI for this closed generic
            var resolved = GetAll(iface);

            resolved.Should().NotBeEmpty($"Handler {handlerType.Name} must be registered in DI");
        }
    }
}
