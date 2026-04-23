using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

using CampFitFurDogs.Api.Tests;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoDuplicateServiceRegistrationGuardrailTests
    : ApiTestBase
{
    public NoDuplicateServiceRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

    [Fact]
    public void Should_Not_Have_Duplicate_DI_Registrations()
    {
        using var scope = Factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        // Get the internal service collection
        var serviceCollection = Factory.Services.GetRequiredService<IEnumerable<ServiceDescriptor>>();

        // Group by service type
        var duplicates = serviceCollection
            .GroupBy(sd => sd.ServiceType)
            .Where(g => g.Count() > 1)
            .Where(g =>
                // Allow multiple implementations ONLY for open generics
                !g.Key.IsGenericTypeDefinition &&
                // Allow multiple handlers for domain events
                !IsDomainEventHandler(g.Key))
            .ToList();

        duplicates.Should().BeEmpty("duplicate DI registrations cause unpredictable runtime behavior");
    }

    private static bool IsDomainEventHandler(Type type)
    {
        if (!type.IsGenericType) return false;

        var def = type.GetGenericTypeDefinition();
        return def.Name.StartsWith("IDomainEventHandler");
    }
}
