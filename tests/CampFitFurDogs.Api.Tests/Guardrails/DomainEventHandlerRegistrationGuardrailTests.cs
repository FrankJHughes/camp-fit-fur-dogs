using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Frank.Abstractions.Events;
using Frank.Testing.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DomainEventHandlerRegistrationGuardrailTests : IAsyncLifetime
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
            .WithCookieAuthOnly(false)
            .WithConfigOverride(cfg =>
                cfg.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Frontend:BaseUrl"] = "http://localhost:5173"
                    }
                )
            );

        return new ApiFactory(ctx);
    }

    private IEnumerable<object> ResolveAll(ApiFactory factory, Type serviceType)
    {
        using var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetServices(serviceType).Cast<object>();
    }

    // ------------------------------------------------------------
    // GUARDRAIL — All domain event handlers must be registered
    // ------------------------------------------------------------
    [Fact]
    public void Should_Register_All_DomainEventHandlers()
    {
        var factory = CreateFactory();

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

        if (handlerTypes.Count == 0)
            return; // nothing to test yet

        foreach (var handlerType in handlerTypes)
        {
            // Find the closed generic interface implemented by this handler
            var iface = handlerType.GetInterfaces()
                .First(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

            // Resolve all registrations for this closed generic
            var resolved = ResolveAll(factory, iface);

            resolved.Should().NotBeEmpty(
                $"Handler {handlerType.Name} must be registered in DI");
        }
    }
}
