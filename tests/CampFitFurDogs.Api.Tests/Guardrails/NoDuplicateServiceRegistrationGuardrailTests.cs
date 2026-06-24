using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoDuplicateServiceRegistrationGuardrailTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    private ApiFactory CreateFactory()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false);

        return new ApiFactory(ctx);
    }

    private static bool IsDomainEventHandler(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var def = type.GetGenericTypeDefinition();
        return def.Name.StartsWith("IDomainEventHandler");
    }

    // ------------------------------------------------------------
    // GUARDRAIL — No duplicate DI registrations
    // ------------------------------------------------------------
    [Fact]
    public void Should_Not_Have_Duplicate_DI_Registrations()
    {
        var factory = CreateFactory();

        using var scope = factory.Services.CreateScope();

        // Pull the internal service collection
        var serviceCollection =
            factory.Services.GetRequiredService<IEnumerable<ServiceDescriptor>>();

        // Group by service type and detect duplicates
        var duplicates = serviceCollection
            .GroupBy(sd => sd.ServiceType)
            .Where(g => g.Count() > 1)
            .Where(g =>
                // Allow multiple implementations ONLY for open generics
                !g.Key.IsGenericTypeDefinition &&
                // Allow multiple domain event handlers
                !IsDomainEventHandler(g.Key))
            .ToList();

        duplicates.Should().BeEmpty(
            "duplicate DI registrations cause unpredictable runtime behavior");
    }
}
